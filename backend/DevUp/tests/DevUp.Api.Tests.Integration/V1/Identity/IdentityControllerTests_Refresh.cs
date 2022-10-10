using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DevUp.Api.Contracts;
using DevUp.Api.Contracts.V1;
using DevUp.Api.Contracts.V1.Identity.Requests;
using DevUp.Api.Contracts.V1.Identity.Responses;
using FluentAssertions;
using Xunit;

namespace DevUp.Api.Tests.Integration.V1.Identity
{
    public class IdentityControllerTests_Refresh : IClassFixture<IdentityApiFactory>
    {
        private IdentityFaker _faker;
        private readonly IdentityApiFactory _apiFactory;
        private readonly HttpClient _apiClient;

        public IdentityControllerTests_Refresh(IdentityApiFactory identityApiFactory)
        {
            _faker = new IdentityFaker();
            _apiFactory = identityApiFactory;
            _apiClient = identityApiFactory.CreateClient();
        }

        [Fact]
        public async Task Refresh_WhenProvidedWithValidTokenPairAfterTokenHasExpired_ReturnsNewTokenPair()
        {
            // register
            await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Register, _faker.RegisterUserRequest);

            // login and grab token pair
            var loginResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Login, _faker.LoginUserRequest);
            var oldTokenPair = await loginResult.Content.ReadFromJsonAsync<IdentityResponse>();

            // wait for token expiration. refresh token will still be active
            await Task.Delay(_apiFactory.AuthenticationOptions.TokenExpiry);

            // refresh and grab new token pair
            var refreshRequest = new RefreshUserRequest()
            {
                Token = oldTokenPair!.Token!,
                RefreshToken = oldTokenPair.RefreshToken!,
                Device = _faker.LoginUserRequest.Device
            };
            var refreshResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Refresh, refreshRequest);
            refreshResult.Should().HaveStatusCode(HttpStatusCode.OK);

            var newTokenPair = await refreshResult.Content.ReadFromJsonAsync<IdentityResponse>();
            newTokenPair!.Token.Should().NotBeEmpty();
            newTokenPair.RefreshToken.Should().NotBeEmpty();
            newTokenPair.Token.Should().NotBe(oldTokenPair.Token);
            newTokenPair.RefreshToken.Should().NotBe(oldTokenPair.RefreshToken);
        }

        [Fact]
        public async Task Refresh_WhenTryingToReuseValidTokenPairOnAnotherDevice_ReturnsBadRequestWithGenericMessage()
        {
            // register
            await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Register, _faker.RegisterUserRequest);

            // login and grab token pair
            var loginResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Login, _faker.LoginUserRequest);
            var oldTokenPair = await loginResult.Content.ReadFromJsonAsync<IdentityResponse>();

            // wait for token expiration. refresh token will still be active
            await Task.Delay(_apiFactory.AuthenticationOptions.TokenExpiry);

            // refresh and grab new token pair
            var differentDevice = new IdentityFaker().LoginUserRequest.Device;
            var refreshRequest = new RefreshUserRequest()
            {
                Token = oldTokenPair!.Token!,
                RefreshToken = oldTokenPair.RefreshToken!,
                Device = differentDevice
            };

            var refreshResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Refresh, refreshRequest);
            refreshResult.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var response = await refreshResult.Content.ReadFromJsonAsync<ErrorResponse>();
            response!.Errors.Should().ContainSingle(e => e == "Invalid request.");
        }

        [Fact]
        public async Task Refresh_WhenProvidedWithInvalidTokenPairThatHasInvalidTokenFormat_ReturnsBadRequestWithDetailedMessage()
        {
            var refreshRequest = new RefreshUserRequest()
            {
                Token = "missing dot separation",
                RefreshToken = "1234abcd!@#$",
                Device = _faker.LoginUserRequest.Device
            };
            var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Refresh, refreshRequest);
            result.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var response = await result.Content.ReadFromJsonAsync<ErrorResponse>();
            response!.Errors.Should().ContainSingle(e => e == "Token is not a valid jwt token.");
        }

        [Fact]
        public async Task Refresh_WhenProvidedWithInvalidTokenPairThatHasValidTokenFormat_ReturnsBadRequestWithGenericMessage()
        {
            var refreshRequest = new RefreshUserRequest()
            {
                Token = "header.payload.signature",
                RefreshToken = "1234abcd!@#$",
                Device = _faker.LoginUserRequest.Device
            };
            var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Refresh, refreshRequest);
            result.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var response = await result.Content.ReadFromJsonAsync<ErrorResponse>();
            response!.Errors.Should().ContainSingle(e => e == "Invalid request.");
        }

        [Fact]
        public async Task Refresh_WhenProvidedWithValidTokenPairAfterBothTokenAndRefreshTokenHaveExpired_ReturnsBadRequestWithGenericMessage()
        {
            // register
            await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Register, _faker.RegisterUserRequest);

            // login and grab token pair
            var loginResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Login, _faker.LoginUserRequest);
            var oldTokenPair = await loginResult.Content.ReadFromJsonAsync<IdentityResponse>();

            // wait for token expiration. both token and refresh token should expire
            var delayMs = new[] { _apiFactory.AuthenticationOptions.TokenExpiry, _apiFactory.AuthenticationOptions.RefreshTokenExpiry }.Max();
            await Task.Delay(delayMs);

            // try refreshing
            var refreshRequest = new RefreshUserRequest()
            {
                Token = oldTokenPair!.Token!,
                RefreshToken = oldTokenPair.RefreshToken!,
                Device = _faker.LoginUserRequest.Device
            };
            var refreshResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Refresh, refreshRequest);
            refreshResult.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var refreshResponse = await refreshResult.Content.ReadFromJsonAsync<ErrorResponse>();
            refreshResponse!.Errors.Should().ContainSingle(e => e == "Invalid request.");
        }
    }
}

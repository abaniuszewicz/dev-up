using System;
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
    public class IdentityControllerTests : IClassFixture<IdentityApiFactory>
    {
        private IdentityFaker _faker;
        private readonly IdentityApiFactory _apiFactory;
        private readonly HttpClient _apiClient;

        public IdentityControllerTests(IdentityApiFactory identityApiFactory)
        {
            _faker = new IdentityFaker();
            _apiFactory = identityApiFactory;
            _apiClient = identityApiFactory.CreateClient();
        }

        [Fact]
        public async Task Register_WhenGivenValidRequest_ReturnsTokenPair()
        {
            var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Register, _faker.RegisterUserRequest);
            result.Should().HaveStatusCode(HttpStatusCode.OK);

            var response = await result.Content.ReadFromJsonAsync<IdentityResponse>();
            response!.Token.Should().NotBeEmpty();
            response.RefreshToken.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Register_WhenGivenInvalidRequest_ReturnsBadRequestWithValidationErrors()
        {
            var invalidRequest = new RegisterUserRequest()
            {
                Username = "BAD-",
                Password = "i'm-G00d-and-$tr0ng-p4ssworD",
                Device = new DeviceRequest() { Id = Guid.NewGuid(), Name = "i'm ok" }
            };

            var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Register, invalidRequest);
            result.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var response = await result.Content.ReadFromJsonAsync<ErrorResponse>();
            var expectedErrors = new[] 
            {
                $"'{nameof(RegisterUserRequest.Username)}' may only contain lowercase letters or hyphens.",
                $"'{nameof(RegisterUserRequest.Username)}' cannot end with a hyphen."
            };
            response!.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public async Task Register_WhenGivenDuplicateUsername_ReturnsBadRequestWithDetailedError()
        {
            var otherFaker = new IdentityFaker();
            var duplicatedUsernameRegisterRequest = new RegisterUserRequest()
            {
                Username = _faker.RegisterUserRequest.Username,
                Password = otherFaker.RegisterUserRequest.Password,
                Device = otherFaker.RegisterUserRequest.Device,
            };

            var nonDuplicatedResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Register, _faker.RegisterUserRequest);
            nonDuplicatedResult.Should().HaveStatusCode(HttpStatusCode.OK);

            var duplicatedResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Register, duplicatedUsernameRegisterRequest);
            duplicatedResult.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var response = await duplicatedResult.Content.ReadFromJsonAsync<ErrorResponse>();
            response!.Errors.Should().ContainSingle(e => e == "User with this username already exist.");
        }

        [Fact]
        public async Task Login_WhenGivenValidCredentialsOfAnExistingUser_ReturnsTokenPair()
        {
            var resultReg = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Register, _faker.RegisterUserRequest);

            var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Login, _faker.LoginUserRequest);
            result.Should().HaveStatusCode(HttpStatusCode.OK);

            var response = await result.Content.ReadFromJsonAsync<IdentityResponse>();
            response!.Token.Should().NotBeEmpty();
            response.RefreshToken.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Login_WhenGivenInvalidCredentials_ReturnsBadRequestWithGenericMessage()
        {
            // try to login without registering
            var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Login, _faker.LoginUserRequest);
            result.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var response = await result.Content.ReadFromJsonAsync<ErrorResponse>();
            response!.Errors.Should().ContainSingle(e => e == "Invalid request.");
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

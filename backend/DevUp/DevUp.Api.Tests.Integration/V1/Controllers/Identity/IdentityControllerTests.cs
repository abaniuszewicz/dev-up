using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DevUp.Api.Contracts.V1.Identity.Requests;
using DevUp.Api.Contracts.V1.Identity.Responses;
using DevUp.Domain.Identity.Exceptions;
using FluentAssertions;
using Xunit;

namespace DevUp.Api.Tests.Integration.V1.Controllers.Identity
{
    public class IdentityControllerTests : IClassFixture<IdentityApiFactory>
    {
        private readonly IdentityApiFactory _apiFactory;
        private readonly HttpClient _apiClient;

        public IdentityControllerTests(IdentityApiFactory identityApiFactory)
        {
            _apiFactory = identityApiFactory;
            _apiClient = identityApiFactory.CreateClient();
        }

        [Fact]
        public async Task Register_WhenGivenValidRequest_ReturnsTokenPair()
        {
            var result = await _apiClient.PostAsJsonAsync("api/v1/identity/register", _apiFactory.SampleRequest.Register);
            var response = await result.Content.ReadFromJsonAsync<IdentityResponse>();

            result.Should().HaveStatusCode(HttpStatusCode.OK);
            response!.Success.Should().BeTrue();
            response.Errors.Should().BeEmpty();
            response.Token.Should().NotBeEmpty();
            response.RefreshToken.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Register_WhenGivenInvalidRequest_ReturnsBadRequest()
        {
            var invalidRequest = new RegisterUserRequest()
            {
                Username = "BAD-",
                Password = "i'm-G00d-and-$tr0ng-p4ssworD",
                Device = new DeviceRequest() { Id = "i'm ok", Name = "i'm ok" }
            };

            var result = await _apiClient.PostAsJsonAsync("api/v1/identity/register", invalidRequest);
            var response = await result.Content.ReadFromJsonAsync<IdentityResponse>();

            result.Should().HaveStatusCode(HttpStatusCode.BadRequest);
            response!.Success.Should().BeFalse();

            var expectedErrors = new[] { UsernameValidationException.InvalidLengthMessage, UsernameValidationException.InvalidCharactersMessage, UsernameValidationException.InvalidFirstOrLastCharacterMessage };
            response.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public async Task Login_WhenGivenValidCredentialsOfAnExistingUser_ReturnsTokenPair()
        {
            var result = await _apiClient.PostAsJsonAsync("api/v1/identity/login", _apiFactory.SampleRequest.Login);
            var response = await result.Content.ReadFromJsonAsync<IdentityResponse>();

            result.Should().HaveStatusCode(HttpStatusCode.OK);
            response!.Success.Should().BeTrue();
            response.Errors.Should().BeEmpty();
            response.Token.Should().NotBeEmpty();
            response.RefreshToken.Should().NotBeEmpty();
        }

        [Fact]
        public async Task Login_WhenGivenInvalidCredentials_ReturnsBadRequest()
        {
            var loginRequest = new LoginUserRequest()
            {
                Username = "i-dont-exist",
                Password = "i'm-G00d-and-$tr0ng-p4ssworD",
                Device = new DeviceRequest() { Id = "i'm ok", Name = "i'm ok" }
            };
            var result = await _apiClient.PostAsJsonAsync("api/v1/identity/login", loginRequest);
            var response = await result.Content.ReadFromJsonAsync<IdentityResponse>();

            result.Should().HaveStatusCode(HttpStatusCode.BadRequest);
            response!.Success.Should().BeFalse();
            response.Errors.Should().ContainSingle(e => e == LoginException.InvalidUsernameMessage);
        }

        [Fact]
        public async Task Refresh_WhenProvidedWithValidTokenPairAfterTokenHasExpired_ReturnsNewTokenPair()
        {
            // login and grab token pair
            var loginResult = await _apiClient.PostAsJsonAsync("api/v1/identity/login", _apiFactory.SampleRequest.Login);
            var oldTokenPair = await loginResult.Content.ReadFromJsonAsync<IdentityResponse>();

            // wait for token expiration. refresh token will still be active
            await Task.Delay(IdentityApiFactory.JWT_EXPIRY_MS);

            // refresh and grab new token pair
            var refreshRequest = new RefreshUserRequest() { Token = oldTokenPair!.Token!, RefreshToken = oldTokenPair.RefreshToken!, Device = _apiFactory.SampleRequest.Login.Device };
            var refreshResult = await _apiClient.PostAsJsonAsync("api/v1/identity/refresh", refreshRequest);
            var newTokenPair = await refreshResult.Content.ReadFromJsonAsync<IdentityResponse>();

            refreshResult.Should().HaveStatusCode(HttpStatusCode.OK);
            newTokenPair!.Success.Should().BeTrue();
            newTokenPair.Errors.Should().BeEmpty();
            newTokenPair.Token.Should().NotBeEmpty();
            newTokenPair.RefreshToken.Should().NotBeEmpty();
            newTokenPair.Token.Should().NotBe(oldTokenPair.Token);
            newTokenPair.RefreshToken.Should().NotBe(oldTokenPair.RefreshToken);
        }

        [Fact]
        public async Task Refresh_WhenProvidedWithInvalidTokenPair_ReturnsBadRequest()
        {
            var refreshRequest = new RefreshUserRequest()
            {
                // this is valid jwt generated online, but it wasn't signed with our app
                Token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c",
                RefreshToken = "refresh-token-is-really-a-random-string",
                Device = new DeviceRequest() { Id = "i'm ok", Name = "i'm ok" }
            };
            var result = await _apiClient.PostAsJsonAsync("api/v1/identity/refresh", refreshRequest);
            var response = await result.Content.ReadFromJsonAsync<IdentityResponse>();

            result.Should().HaveStatusCode(HttpStatusCode.BadRequest);
            response!.Success.Should().BeFalse();
            var expectedErrors = new[] { RefreshException.InvalidTokenMessage, RefreshException.InvalidRefreshTokenMessage };
            response.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public async Task Refresh_WhenProvidedWithValidTokenPairAfterBothTokenAndRefreshTokenHaveExpired_ReturnsBadRequest()
        {
            // login and grab token pair
            var loginResult = await _apiClient.PostAsJsonAsync("api/v1/identity/login", _apiFactory.SampleRequest.Login);
            var oldTokenPair = await loginResult.Content.ReadFromJsonAsync<IdentityResponse>();

            // wait for token expiration. both token and refresh token should expire
            var delayMs = Math.Max(IdentityApiFactory.JWT_EXPIRY_MS, IdentityApiFactory.JWT_REFRESH_EXPIRY_MS);
            await Task.Delay(delayMs);

            // try refreshing
            var refreshRequest = new RefreshUserRequest() { Token = oldTokenPair!.Token!, RefreshToken = oldTokenPair.RefreshToken!, Device = _apiFactory.SampleRequest.Login.Device };
            var refreshResult = await _apiClient.PostAsJsonAsync("api/v1/identity/refresh", refreshRequest);
            var refreshResponse = await refreshResult.Content.ReadFromJsonAsync<IdentityResponse>();

            refreshResult.Should().HaveStatusCode(HttpStatusCode.BadRequest);
            refreshResponse!.Success.Should().BeFalse();
            refreshResponse.Errors.Should().ContainSingle(e => e == TokenValidationException.RefreshTokenNotActiveMessage);
        }
    }
}

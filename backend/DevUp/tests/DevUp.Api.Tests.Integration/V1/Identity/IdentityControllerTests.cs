using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DevUp.Api.Contracts;
using DevUp.Api.Contracts.V1;
using DevUp.Api.Contracts.V1.Identity.Requests;
using DevUp.Api.Contracts.V1.Identity.Responses;
using DevUp.Domain.Identity.Services.Exceptions;
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
            var response = await result.Content.ReadFromJsonAsync<IdentityResponse>();

            result.Should().HaveStatusCode(HttpStatusCode.OK);
            response!.Token.Should().NotBeEmpty();
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

            var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Register, invalidRequest);
            var response = await result.Content.ReadFromJsonAsync<ErrorResponse>();

            result.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var expectedErrors = new[] 
            {
                $"'{nameof(RegisterUserRequest.Username)}' must be between 6 and 30 characters. You entered 4 characters.",
                $"'{nameof(RegisterUserRequest.Username)}' may only contain lowercase letters or hyphens.",
                $"'{nameof(RegisterUserRequest.Username)}' cannot end with a hyphen."
            };
            response!.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public async Task Login_WhenGivenValidCredentialsOfAnExistingUser_ReturnsTokenPair()
        {
            for (var i = 0; i < 10_000; i++)
            {
                var faker = new IdentityFaker();
                var resultReg = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Register, faker.RegisterUserRequest);

                var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Login, faker.LoginUserRequest);
                var response = await result.Content.ReadFromJsonAsync<IdentityResponse>();

                if (!result.IsSuccessStatusCode)
                {
                    var decode = resultReg.Content.ReadAsStringAsync();
                }

                result.Should().HaveStatusCode(HttpStatusCode.OK);
                response!.Token.Should().NotBeEmpty();
                response.RefreshToken.Should().NotBeEmpty();

                await Task.Delay(1);
            }
        }

        [Fact]
        public async Task Check()
        {
            var register = new RegisterUserRequest()
            {
                Username = "jeannette-shields",
                Password = "lowUPP1$MMYVy",
                Device = new DeviceRequest()
                {
                    Id = "2d18c462-fbd9-ddde-4b1b-932761c7fda9",
                    Name = "PC"
                }
            };

            var login = new LoginUserRequest()
            {
                Username = register.Username,
                Password = register.Password,
                Device = new DeviceRequest()
                {
                    Id = register.Device.Id,
                    Name = register.Device.Name
                }
            };
            
            await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Register, register);

            var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Login, login);
            var response = await result.Content.ReadFromJsonAsync<IdentityResponse>();

            if (!result.IsSuccessStatusCode)
            {
            }

            result.Should().HaveStatusCode(HttpStatusCode.OK);
            response!.Token.Should().NotBeEmpty();
            response.RefreshToken.Should().NotBeEmpty();

            await Task.Delay(1);
        }

        [Fact]
        public async Task Login_WhenGivenInvalidCredentials_ReturnsBadRequest()
        {
            // try to login without registering
            var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Login, _faker.LoginUserRequest);
            var response = await result.Content.ReadFromJsonAsync<ErrorResponse>();

            result.Should().HaveStatusCode(HttpStatusCode.BadRequest);
            response!.Errors.Should().ContainSingle(e => e == LoginException.InvalidUsernameMessage);
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
            await Task.Delay(IdentityApiFactory.JWT_EXPIRY_MS + 1);

            // refresh and grab new token pair
            var refreshRequest = new RefreshUserRequest() 
            { 
                Token = oldTokenPair!.Token!, 
                RefreshToken = oldTokenPair.RefreshToken!, 
                Device = _faker.LoginUserRequest.Device 
            };
            var refreshResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Refresh, refreshRequest);
            var newTokenPair = await refreshResult.Content.ReadFromJsonAsync<IdentityResponse>();

            refreshResult.Should().HaveStatusCode(HttpStatusCode.OK);
            newTokenPair!.Token.Should().NotBeEmpty();
            newTokenPair.RefreshToken.Should().NotBeEmpty();
            newTokenPair.Token.Should().NotBe(oldTokenPair.Token);
            newTokenPair.RefreshToken.Should().NotBe(oldTokenPair.RefreshToken);
        }

        [Fact]
        public async Task Refresh_WhenProvidedWithInvalidTokenPair_ReturnsBadRequest()
        {
            var refreshRequest = new RefreshUserRequest()
            {
                Token = "header.payload.signature",
                RefreshToken = "1234abcd!@#$",
                Device = _faker.LoginUserRequest.Device
            };
            var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Refresh, refreshRequest);
            var response = await result.Content.ReadFromJsonAsync<ErrorResponse>();

            result.Should().HaveStatusCode(HttpStatusCode.BadRequest);
            var expectedErrors = new[] { RefreshException.InvalidTokenMessage, RefreshException.InvalidRefreshTokenMessage };
            response!.Errors.Should().BeEquivalentTo(expectedErrors);
        }

        [Fact]
        public async Task Refresh_WhenProvidedWithValidTokenPairAfterBothTokenAndRefreshTokenHaveExpired_ReturnsBadRequest()
        {
            // register
            await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Register, _faker.RegisterUserRequest);

            // login and grab token pair
            var loginResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Login, _faker.LoginUserRequest);
            var oldTokenPair = await loginResult.Content.ReadFromJsonAsync<IdentityResponse>();

            // wait for token expiration. both token and refresh token should expire
            var delayMs = Math.Max(IdentityApiFactory.JWT_EXPIRY_MS, IdentityApiFactory.JWT_REFRESH_EXPIRY_MS) + 1;
            await Task.Delay(delayMs);

            // try refreshing
            var refreshRequest = new RefreshUserRequest() 
            { 
                Token = oldTokenPair!.Token!, 
                RefreshToken = oldTokenPair.RefreshToken!, 
                Device = _faker.LoginUserRequest.Device 
            };
            var refreshResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Refresh, refreshRequest);
            var refreshResponse = await refreshResult.Content.ReadFromJsonAsync<ErrorResponse>();

            refreshResult.Should().HaveStatusCode(HttpStatusCode.BadRequest);
            refreshResponse!.Errors.Should().ContainSingle(e => e == TokenValidationException.RefreshTokenNotActiveMessage);
        }
    }
}

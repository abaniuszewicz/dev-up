﻿using System.Net;
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
    public class IdentityControllerTests_Register : IClassFixture<IdentityApiFactory>
    {
        private IdentityFaker _faker;
        private readonly IdentityApiFactory _apiFactory;
        private readonly HttpClient _apiClient;

        public IdentityControllerTests_Register(IdentityApiFactory identityApiFactory)
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
                Password = _faker.RegisterUserRequest.Password,
                Device = _faker.RegisterUserRequest.Device
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
    }
}

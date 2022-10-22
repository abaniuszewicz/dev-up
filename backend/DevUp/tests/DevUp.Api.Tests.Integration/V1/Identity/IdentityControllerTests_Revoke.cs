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
    public class IdentityControllerTests_Revoke : IClassFixture<IdentityApiFactory>
    {
        private IdentityFaker _faker;
        private readonly IdentityApiFactory _apiFactory;
        private readonly HttpClient _apiClient;

        public IdentityControllerTests_Revoke(IdentityApiFactory identityApiFactory)
        {
            _faker = new IdentityFaker();
            _apiFactory = identityApiFactory;
            _apiClient = identityApiFactory.CreateClient();
        }

        [Fact]
        public async Task Revoke_WhenTryingToRevokeRefreshTokenThatDoesNotExist_ReturnsBadRequestWithGenericMessage()
        {
            var request = new RevokeTokenRequest() { RefreshToken = "i don't exist" };

            var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Revoke, request);
            result.Should().HaveStatusCode(HttpStatusCode.BadRequest);

            var response = await result.Content.ReadFromJsonAsync<ErrorResponse>();
            response!.Errors.Should().ContainSingle(e => e == "Invalid request.");
        }

        [Fact]
        public async Task Revoke_WhenTryingToRevokeRefreshTokenThatDoesExist_ReturnsOk()
        {
            // register
            var registerResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Register, _faker.RegisterUserRequest);
            var tokenPair = await registerResult.Content.ReadFromJsonAsync<IdentityResponse>();

            var request = new RevokeTokenRequest() { RefreshToken = tokenPair!.RefreshToken };

            var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Identity.Revoke, request);
            result.Should().HaveStatusCode(HttpStatusCode.OK);
        }
    }
}

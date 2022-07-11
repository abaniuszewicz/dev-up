using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DevUp.Api.V1.Controllers.Identity.Requests;
using DevUp.Api.V1.Controllers.Identity.Responses;
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
    }
}

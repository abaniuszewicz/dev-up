using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DevUp.Api.Contracts;
using DevUp.Api.Contracts.V1;
using DevUp.Api.Contracts.V1.Identity.Responses;
using FluentAssertions;
using Xunit;

namespace DevUp.Api.Tests.Integration.V1.Identity
{
    public class IdentityControllerTests_Login : IClassFixture<IdentityApiFactory>
    {
        private IdentityFaker _faker;
        private readonly IdentityApiFactory _apiFactory;
        private readonly HttpClient _apiClient;

        public IdentityControllerTests_Login(IdentityApiFactory identityApiFactory)
        {
            _faker = new IdentityFaker();
            _apiFactory = identityApiFactory;
            _apiClient = identityApiFactory.CreateClient();
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
    }
}

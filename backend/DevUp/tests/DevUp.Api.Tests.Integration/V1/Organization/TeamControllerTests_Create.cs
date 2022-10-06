using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DevUp.Api.Contracts;
using DevUp.Api.Contracts.V1.Organization.Requests;
using DevUp.Api.Contracts.V1.Organization.Responses;
using FluentAssertions;
using Xunit;

namespace DevUp.Api.Tests.Integration.V1.Organization
{
    public class TeamControllerTests_Create : IClassFixture<TeamApiFactory>
    {
        private readonly OrganizationFaker _faker;
        private readonly HttpClient _apiClient;

        public TeamControllerTests_Create(TeamApiFactory teamApiFactory)
        {
            _faker = new OrganizationFaker();
            _apiClient = teamApiFactory.CreateClient();
        }

        [Fact]
        public async Task Create_WhenGivenValidRequest_CreatesTeam()
        {
            var createdResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Teams.Create, _faker.CreateTeamRequest);
            createdResult.Should().HaveStatusCode(HttpStatusCode.Created);

            var location = createdResult.Headers.Location!;
            var retrievedResult = await _apiClient.GetAsync(location);
            retrievedResult.Should().HaveStatusCode(HttpStatusCode.OK);

            var retrievedResponse = await retrievedResult.Content.ReadFromJsonAsync<TeamResponse>();
            retrievedResponse!.Id.Should().Be(location.Segments.Last());
            retrievedResponse.Name.Should().Be(_faker.CreateTeamRequest.Name);
        }

        [Fact]
        public async Task Create_WhenGivenInvalidRequest_ReturnsBadRequest()
        {
            var team = new CreateTeamRequest() { Name = "******Bad nam3-----???" };

            var result = await _apiClient.PostAsJsonAsync(Route.Api.V1.Teams.Create, team);
            result.Should().HaveStatusCode(HttpStatusCode.BadRequest);
        }
    }
}

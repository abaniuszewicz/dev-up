using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using DevUp.Api.Contracts;
using DevUp.Api.Contracts.V1.Organization.Responses;
using FluentAssertions;
using Xunit;

namespace DevUp.Api.Tests.Integration.V1.Organization
{
    public class TeamControllerTests_GetAll : IClassFixture<TeamApiFactory>
    {
        private readonly OrganizationFaker _faker;
        private readonly HttpClient _apiClient;

        public TeamControllerTests_GetAll(TeamApiFactory teamApiFactory)
        {
            _faker = new OrganizationFaker();
            _apiClient = teamApiFactory.CreateClient();
        }

        [Fact]
        public async Task GetAll_WhenThereAreNoTeams_ReturnsEmptyArray()
        {
            var result = await _apiClient.GetAsync(Route.Api.V1.Teams.GetAll);

            result.Should().HaveStatusCode(HttpStatusCode.OK);
            var response = await result.Content.ReadFromJsonAsync<IEnumerable<TeamResponse>>();
            response.Should().BeEmpty();
        }

        [Fact]
        public async Task GetAll_WhenCalled_ReturnsAllExistingTeams()
        {
            var teams = Enumerable.Range(0, 3).Select(_ => new OrganizationFaker().CreateTeamRequest).ToArray();
            foreach (var team in teams)
            {
                var createdResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Teams.Create, team);
                createdResult.Should().HaveStatusCode(HttpStatusCode.Created);
            }

            var result = await _apiClient.GetAsync(Route.Api.V1.Teams.GetAll);

            result.Should().HaveStatusCode(HttpStatusCode.OK);
            var response = await result.Content.ReadFromJsonAsync<IEnumerable<TeamResponse>>();
            response.Should().NotBeEmpty();
            response.Should().HaveCountGreaterOrEqualTo(teams.Length);
            teams.Select(t => t.Name).Should().BeSubsetOf(response!.Select(r => r.Name));

            // cleanup for other tests
            foreach (var team in response!)
            {
                var url = Route.Api.V1.Teams.DeleteFactory(team.Id);
                var deleteResult = await _apiClient.DeleteAsync(url);
                deleteResult.Should().HaveStatusCode(HttpStatusCode.NoContent);
            }
        }
    }
}

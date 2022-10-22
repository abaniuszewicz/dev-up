using System;
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
    public class TeamControllerTests_Delete : IClassFixture<TeamApiFactory>
    {
        private readonly OrganizationFaker _faker;
        private readonly HttpClient _apiClient;

        public TeamControllerTests_Delete(TeamApiFactory teamApiFactory)
        {
            _faker = new OrganizationFaker();
            _apiClient = teamApiFactory.CreateClient();
        }

        [Fact]
        public async Task Delete_WhenGivenNonExistingId_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            var result = await _apiClient.DeleteAsync(Route.Api.V1.Teams.DeleteFactory(id));
            result.Should().HaveStatusCode(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_WhenGivenExistingId_DeletesTeam()
        {
            var createdResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Teams.Create, _faker.CreateTeamRequest);
            createdResult.Should().HaveStatusCode(HttpStatusCode.Created);

            var retrievedResult = await _apiClient.GetAsync(createdResult.Headers.Location);
            retrievedResult.Should().HaveStatusCode(HttpStatusCode.OK);
            var retrievedResponse = await retrievedResult.Content.ReadFromJsonAsync<TeamResponse>();

            var deletedResult = await _apiClient.DeleteAsync(Route.Api.V1.Teams.DeleteFactory(retrievedResponse!.Id));
            deletedResult.Should().HaveStatusCode(HttpStatusCode.NoContent);
        }
    }
}

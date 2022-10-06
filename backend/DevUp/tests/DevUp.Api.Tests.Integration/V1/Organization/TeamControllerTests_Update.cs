using System;
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
    public class TeamControllerTests_Update : IClassFixture<TeamApiFactory>
    {
        private readonly OrganizationFaker _faker;
        private readonly HttpClient _apiClient;

        public TeamControllerTests_Update(TeamApiFactory teamApiFactory)
        {
            _faker = new OrganizationFaker();
            _apiClient = teamApiFactory.CreateClient();
        }

        [Fact]
        public async Task Update_WhenGivenNonExistingId_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            var result = await _apiClient.PutAsJsonAsync(Route.Api.V1.Teams.UpdateFactory(id), _faker.UpdateTeamRequest);
            result.Should().HaveStatusCode(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Update_WhenGivenExistingIdWithInvalidRequest_ReturnsBadRequest()
        {
            var createdResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Teams.Create, _faker.CreateTeamRequest);
            createdResult.Should().HaveStatusCode(HttpStatusCode.Created);

            var id = Guid.Parse(createdResult.Headers.Location!.Segments.Last());
            var updateResult = await _apiClient.PutAsJsonAsync(Route.Api.V1.Teams.UpdateFactory(id), new UpdateTeamRequest() { Name = "**InVAL1D--=" });
            updateResult.Should().HaveStatusCode(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Update_WhenGivenExistingIdWithValidRequest_UpdatesTeam()
        {
            var createdResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Teams.Create, _faker.CreateTeamRequest);
            createdResult.Should().HaveStatusCode(HttpStatusCode.Created);

            var id = Guid.Parse(createdResult.Headers.Location!.Segments.Last());
            var updatedResult = await _apiClient.PutAsJsonAsync(Route.Api.V1.Teams.UpdateFactory(id), _faker.UpdateTeamRequest);
            updatedResult.Should().HaveStatusCode(HttpStatusCode.OK);

            var retrieveResult = await _apiClient.GetAsync(createdResult.Headers.Location);
            retrieveResult.Should().HaveStatusCode(HttpStatusCode.OK);
            var response = await retrieveResult.Content.ReadFromJsonAsync<TeamResponse>();

            response!.Name.Should().NotBe(_faker.CreateTeamRequest.Name);
            response.Name.Should().Be(_faker.UpdateTeamRequest.Name);
        }
    }
}

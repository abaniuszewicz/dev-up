using System;
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
    public class TeamControllerTests_GetById : IClassFixture<TeamApiFactory>
    {
        private readonly OrganizationFaker _faker;
        private readonly HttpClient _apiClient;

        public TeamControllerTests_GetById(TeamApiFactory teamApiFactory)
        {
            _faker = new OrganizationFaker();
            _apiClient = teamApiFactory.CreateClient();
        }

        [Fact]
        public async Task GetById_WhenGivenNonExistingId_ReturnsNotFound()
        {
            var id = Guid.NewGuid();

            var result = await _apiClient.GetAsync(Route.Api.V1.Teams.GetByIdFactory(id));

            result.Should().HaveStatusCode(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetById_WhenGivenExistingId_ReturnsTeamWithThatId()
        {
            var createdResult = await _apiClient.PostAsJsonAsync(Route.Api.V1.Teams.Create, _faker.CreateTeamRequest);
            var createdId = createdResult.Headers.Location!.Segments.Last();

            var result = await _apiClient.GetAsync(createdResult.Headers.Location);

            result.Should().HaveStatusCode(HttpStatusCode.OK);
            var response = await result.Content.ReadFromJsonAsync<TeamResponse>();
            response!.Id.Should().Be(createdId);
            response.Name.Should().Be(_faker.CreateTeamRequest.Name);
        }
    }
}

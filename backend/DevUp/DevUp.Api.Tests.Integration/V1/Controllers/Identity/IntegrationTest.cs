using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DevUp.Api.Tests.Integration.V1.Controllers.Identity
{
    public class IntegrationTest : IClassFixture<IdentityApiFactory>, IAsyncLifetime
    {
        private readonly IdentityApiFactory _apiFactory;

        public IntegrationTest(IdentityApiFactory apiFactory)
        {
            _apiFactory = apiFactory;
        }

        [Fact]
        public async Task Test()
        {
            using var client = _apiFactory.CreateClient();
            var content = new StringContent("{ \"username\": \"johnny-sins\", \"password\": \"secret-pass\"");
            var result = await client.PostAsync("/identity/register", content);

        }

        public Task InitializeAsync()
        {
            return Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await _apiFactory.DisposeAsync();
        }
    }
}

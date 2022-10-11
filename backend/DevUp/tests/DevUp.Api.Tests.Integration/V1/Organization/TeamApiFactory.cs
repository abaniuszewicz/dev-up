using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using DevUp.Infrastructure.Postgres.Migrations;
using DevUp.Api.Tests.Integration.Common;
using DevUp.Infrastructure.Postgres.Setup;

namespace DevUp.Api.Tests.Integration.V1.Organization
{
    public class TeamApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {
        private readonly TestcontainerDatabase _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration("postgres:latest")
            {
                Database = "test_postgres",
                Username = "test_postgres",
                Password = "test_pass"
            })
            .Build();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(l => l.ClearProviders());
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IDbConnectionFactory>();
                services.AddSingleton<IDbConnectionFactory>(new TestcontainersDbConnectionFactory(_dbContainer));
            });

            base.ConfigureWebHost(builder);

        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            return base.CreateHost(builder).MigrateUp();
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
        }
    }
}

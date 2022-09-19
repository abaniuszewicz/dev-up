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
using System;
using Microsoft.Extensions.Hosting;
using DevUp.Infrastructure.Postgres.Migrations;
using DevUp.Domain.Organization.Repositories;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.ValueObjects;
using System.Threading;
using DevUp.Api.Tests.Integration.Common;
using DevUp.Infrastructure.Postgres.Setup;

namespace DevUp.Api.Tests.Integration.V1.Organization
{
    public class TeamApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {
        private readonly TestcontainerDatabase _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration()
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
                services.AddSingleton<IDbConnectionFactory>(new TestcontainerDbConnectionFactory(_dbContainer));
            });

            base.ConfigureWebHost(builder);

        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            var host = base.CreateHost(builder).MigrateUp();

            using var scope = host.Services.CreateScope();
            var teamRepository = scope.ServiceProvider.GetRequiredService<ITeamRepository>();

            var teamToCreate = new Team(new TeamId(Guid.NewGuid()), new TeamName("whatever"));
            var createdTeam = teamRepository.CreateAsync(teamToCreate, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            var retrievedTeam = teamRepository.GetByIdAsync(teamToCreate.Id, CancellationToken.None).ConfigureAwait(false).GetAwaiter().GetResult();
            Assert.Equal(createdTeam, retrievedTeam);

            return host;
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

using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Moq;
using DevUp.Domain.Common.Services;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using Microsoft.Extensions.Hosting;
using DevUp.Infrastructure.Postgres.Migrations;
using System.Collections.Generic;
using DevUp.Api.Contracts.V1.Organization.Requests;
using DevUp.Domain.Organization.Repositories;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.ValueObjects;
using System.Threading;

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
            var environmentVariables = new Dictionary<string, string>()
            {
                { "JWT_SECRET", "random-30-char-long-secret-key" },
                { "JWT_EXPIRY_MS", "1000" },
                { "JWT_REFRESH_EXPIRY_MS", "5000" },
                { "DB_CONNECTION_STRING", _dbContainer.ConnectionString },
            };

            var secretProvider = new Mock<ISecretProvider>();
            foreach ((var name, var value) in environmentVariables)
            {
                secretProvider.Setup(sp => sp.Get(name)).Returns(value);
                Environment.SetEnvironmentVariable(name, value);
            }

            builder.ConfigureLogging(l => l.ClearProviders());
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<ISecretProvider>();
                services.AddSingleton<ISecretProvider>(secretProvider.Object);
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

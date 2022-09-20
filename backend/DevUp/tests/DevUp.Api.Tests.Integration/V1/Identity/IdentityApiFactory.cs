using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using DotNet.Testcontainers.Configurations;
using Microsoft.Extensions.Hosting;
using DevUp.Infrastructure.Postgres.Migrations;
using Microsoft.Extensions.DependencyInjection.Extensions;
using DevUp.Infrastructure.Postgres.Setup;
using Microsoft.Extensions.DependencyInjection;
using DevUp.Api.Tests.Integration.Common;
using DevUp.Domain.Identity.Setup;
using Bogus;
using System;

namespace DevUp.Api.Tests.Integration.V1.Identity
{
    public class IdentityApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {
        private static readonly Faker<AuthenticationOptions> AuthenticationOptionsFaker = new Faker<AuthenticationOptions>()
               .RuleFor(ao => ao.TokenExpiry, f => f.Date.BetweenTimeOnly(new TimeOnly(0, 0, 1), new TimeOnly(0, 0, 2)).ToTimeSpan())
               .RuleFor(ao => ao.RefreshTokenExpiry, (f, ao) => 5 * ao.TokenExpiry)
               .RuleFor(ao => ao.SigningKey, f => f.Random.String2(32));

        private readonly TestcontainerDatabase _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration()
            {
                Database = "test_postgres",
                Username = "test_postgres",
                Password = "test_pass"
            })
            .Build();

        public AuthenticationOptions AuthenticationOptions { get; private set; }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(l => l.ClearProviders());
            builder.ConfigureTestServices(services =>
            {
                services.RemoveAll<IDbConnectionFactory>();
                services.AddSingleton<IDbConnectionFactory>(new TestcontainerDbConnectionFactory(_dbContainer));

                services.PostConfigure<AuthenticationOptions>(ao =>
                {
                    AuthenticationOptionsFaker.Populate(ao);
                    AuthenticationOptions = ao;
                });
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

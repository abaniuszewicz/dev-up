using System;
using System.Threading.Tasks;
using Bogus;
using DevUp.Domain.Identity.Setup;
using DevUp.Domain.Tests.Integration.Common;
using DevUp.Infrastructure.Postgres;
using DevUp.Infrastructure.Postgres.Migrations;
using DevUp.Infrastructure.Postgres.Setup;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Configurations;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit;

namespace DevUp.Domain.Tests.Integration.Identity.Services
{
    public class TokenServiceFactory : ApplicationFactory, IAsyncLifetime
    {
        private static readonly Faker<AuthenticationOptions> AuthenticationOptionsFaker = new Faker<AuthenticationOptions>()
               .RuleFor(ao => ao.TokenExpiry, RandomTimespan)
               .RuleFor(ao => ao.RefreshTokenExpiry, (f, ao) => 5 * ao.TokenExpiry)
               .RuleFor(ao => ao.SigningKey, f => f.Random.String2(32));

        private readonly Faker _faker = new Faker();
        private readonly TestcontainerDatabase _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration("postgres:latest")
            {
                Database = "test_postgres",
                Username = "test_postgres",
                Password = "test_pass"
            })
            .Build();

        public AuthenticationOptions AuthenticationOptions { get; private set; }

        public TService Create<TService>()
            where TService : notnull
        {
            return Host.Services.GetRequiredService<TService>();
        }

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();
            Host.MigrateUp();
        }

        public async Task DisposeAsync()
        {
            await Host.StopAsync();
            await _dbContainer.DisposeAsync();
        }

        protected override IServiceCollection ConfigureServices(IServiceCollection services)
        {
            services.AddDomain(Configuration);
            services.AddPostgresInfrastructure(Configuration);

            services.RemoveAll<IDbConnectionFactory>();
            services.AddTransient<IDbConnectionFactory>(_ => new TestcontainersDbConnectionFactory(_dbContainer));
            services.PostConfigure<AuthenticationOptions>(ao =>
            {
                AuthenticationOptionsFaker.Populate(ao);
                AuthenticationOptions = ao;
            });

            return services;
        }

        private static TimeSpan RandomTimespan(Faker faker)
        {
            var min = TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(1));
            var max = TimeOnly.FromTimeSpan(TimeSpan.FromSeconds(2));
            return faker.Date.BetweenTimeOnly(min, max).ToTimeSpan();
        }
    }
}

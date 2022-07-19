using System.Threading.Tasks;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;
using Microsoft.AspNetCore.TestHost;
using DotNet.Testcontainers.Configurations;
using System;
using Microsoft.Extensions.Hosting;
using DevUp.Infrastructure.Postgres.Migrations;
using System.Globalization;

namespace DevUp.Api.Tests.Integration.V1.Controllers.Identity
{
    public class IdentityApiFactory : WebApplicationFactory<IApiMarker>, IAsyncLifetime
    {
        public const int JWT_EXPIRY_MS = 1000;
        public const int JWT_REFRESH_EXPIRY_MS = 5000;

        private readonly TestcontainerDatabase _dbContainer = new TestcontainersBuilder<PostgreSqlTestcontainer>()
            .WithDatabase(new PostgreSqlTestcontainerConfiguration()
            {
                Database = "test_postgres",
                Username = "test_postgres",
                Password = "test_pass"
            })
            .Build();

        public SampleRequest SampleRequest { get; } = new SampleRequest(); 

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureLogging(l => l.ClearProviders());
            builder.ConfigureTestServices(services =>
            {
                // swap any registered services here (.RemoveAll<T>() + AddXyz<T>(fake))
            });

            Environment.SetEnvironmentVariable("JWT_SECRET", "random-30-char-long-secret-key");
            Environment.SetEnvironmentVariable("JWT_EXPIRY_MS", JWT_EXPIRY_MS.ToString(CultureInfo.InvariantCulture));
            Environment.SetEnvironmentVariable("JWT_REFRESH_EXPIRY_MS", JWT_REFRESH_EXPIRY_MS.ToString(CultureInfo.InvariantCulture));
            Environment.SetEnvironmentVariable("DB_CONNECTION_STRING", _dbContainer.ConnectionString);

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

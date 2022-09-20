using DevUp.Infrastructure.Postgres.Identity;
using DevUp.Infrastructure.Postgres.Migrations;
using DevUp.Infrastructure.Postgres.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Infrastructure.Postgres
{
    public static class PostgresInfrastructureInstaller
    {
        public static IServiceCollection AddPostgresInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddPostgresIdentity();
            services.AddDatabaseMigrator();

            services.Configure<PostgresOptions>(configuration.GetRequiredSection("Postgres"));
            services.AddTransient<IDbConnectionFactory, PostgresConnectionFactory>();
            services.AddAutoMapper(typeof(PostgresInfrastructureInstaller).Assembly);
            return services;
        }
    }
}

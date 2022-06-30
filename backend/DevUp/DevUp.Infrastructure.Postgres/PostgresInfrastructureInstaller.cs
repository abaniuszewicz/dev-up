using System.Data;
using DevUp.Infrastructure.Postgres.Identity;
using DevUp.Infrastructure.Postgres.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace DevUp.Infrastructure.Postgres
{
    public static class PostgresInfrastructureInstaller
    {
        public static IServiceCollection AddPostgresInfrastructure(this IServiceCollection services)
        {
            var settings = new PostgresSettings();
            services.AddSingleton(settings);

            services.AddPostgresIdentity();
            services.AddDatabaseMigrator();

            services.AddTransient<IDbConnection>(s => new NpgsqlConnection(settings.ConnectionString));
            services.AddAutoMapper(typeof(PostgresInfrastructureInstaller).Assembly);
            return services;
        }
    }
}

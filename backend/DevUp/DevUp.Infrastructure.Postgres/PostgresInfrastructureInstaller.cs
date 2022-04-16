using System.Data;
using Microsoft.Data.SqlClient;
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

            services.AddTransient<IDbConnection>(s => new NpgsqlConnection(settings.ConnectionString));
            return services;
        }
    }
}

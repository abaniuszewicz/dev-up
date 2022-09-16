using DevUp.Infrastructure.Postgres.Identity;
using DevUp.Infrastructure.Postgres.Migrations;
using DevUp.Infrastructure.Postgres.Organization;
using DevUp.Infrastructure.Postgres.Setup;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Infrastructure.Postgres
{
    public static class PostgresInfrastructureInstaller
    {
        public static IServiceCollection AddPostgresInfrastructure(this IServiceCollection services)
        {
            services.AddPostgresIdentity();
            services.AddPostgresOrganization();
            services.AddDatabaseMigrator();

            services.AddTransient<IDbConnectionFactory, PostgresConnectionFactory>();
            services.AddAutoMapper(typeof(IPostgresInfrastructureMarker).Assembly);
            services.AddMediatR(typeof(IPostgresInfrastructureMarker));
            return services;
        }
    }
}

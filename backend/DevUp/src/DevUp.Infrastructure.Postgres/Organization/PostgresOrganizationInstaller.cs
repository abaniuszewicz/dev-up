using DevUp.Domain.Organization.Repositories;
using DevUp.Infrastructure.Postgres.Organization.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Infrastructure.Postgres.Organization
{
    internal static class PostgresOrganizationInstaller
    {
        public static IServiceCollection AddPostgresOrganization(this IServiceCollection services)
        {
            services.AddScoped<ITeamRepository, TeamRepository>();
            services.AddScoped<IMemberRepository, MemberRepository>();
            return services;
        }
    }
}

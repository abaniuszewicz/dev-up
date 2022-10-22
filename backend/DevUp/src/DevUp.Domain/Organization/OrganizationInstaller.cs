using DevUp.Domain.Organization.Services;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Domain.Organization
{
    internal static class OrganizationInstaller
    {
        public static IServiceCollection AddOrganization(this IServiceCollection services)
        {
            services.AddScoped<ITeamService, TeamService>();
            return services;
        }
    }
}

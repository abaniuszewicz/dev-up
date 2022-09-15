using DevUp.Domain.Identity;
using DevUp.Domain.Organization;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Domain
{
    public static class DomainInstaller
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddIdentity();
            services.AddOrganization();
            return services;
        }
    }
}

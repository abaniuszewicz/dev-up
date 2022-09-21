using DevUp.Domain.Identity;
using DevUp.Domain.Organization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Domain
{
    public static class DomainInstaller
    {
        public static IServiceCollection AddDomain(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity(configuration);
            services.AddOrganization();
            return services;
        }
    }
}

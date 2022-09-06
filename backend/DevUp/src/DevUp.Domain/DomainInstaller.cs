using DevUp.Domain.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Domain
{
    public static class DomainInstaller
    {
        public static IServiceCollection AddDomain(this IServiceCollection services)
        {
            services.AddIdentity();
            return services;
        }
    }
}

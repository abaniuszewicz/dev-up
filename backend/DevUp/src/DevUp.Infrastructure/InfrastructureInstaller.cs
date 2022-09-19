using DevUp.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Infrastructure
{
    public static class InfrastructureInstaller
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddLogger();
            return services;
        }
    }
}

using DevUp.Infrastructure.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Infrastructure
{
    public static class InfrastructureInstaller
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            var secretProvider = new EnvSecretProvider();
            services.AddSingleton<ISecretProvider>(secretProvider);

            services.AddLogger();
            services.AddIdentity(secretProvider);
            return services;
        }
    }
}

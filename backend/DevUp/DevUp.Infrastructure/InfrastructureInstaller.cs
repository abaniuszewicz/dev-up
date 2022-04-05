using DevUp.Infrastructure.System;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DevUp.Infrastructure
{
    public static class InfrastructureInstaller
    {
        public static IServiceCollection AddDefaultInfrastructure(this IServiceCollection services)
        {
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            services.AddSingleton<IEnvironmentVariableRetriever, EnvironmentVariableRetriever>();
            return services;
        }
    }
}

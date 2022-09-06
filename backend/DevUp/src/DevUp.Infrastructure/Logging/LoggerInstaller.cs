using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace DevUp.Infrastructure.Logging
{
    internal static class LoggerInstaller
    {
        public static IServiceCollection AddLogger(this IServiceCollection services)
        {
            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            return services;
        }
    }
}

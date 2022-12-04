using System;
using DevUp.Infrastructure.Logging.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Sinks.Elasticsearch;

namespace DevUp.Infrastructure.Logging
{
    internal static class LoggerInstaller
    {
        public static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetRequiredSection("Elasticsearch").Get<ElasticsearchOptions>();
            var sinkOptions = new ElasticsearchSinkOptions(options.Uri)
            {
                IndexFormat = $"devup-logs-{DateTime.UtcNow:yyyy-MM}",
                AutoRegisterTemplate = true
            };

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(sinkOptions)
                .CreateLogger();

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            return services;
        }
    }
}

using System.Linq;
using DevUp.Infrastructure.Logging.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Filters;
using Serilog.Sinks.Elasticsearch;

namespace DevUp.Infrastructure.Logging
{
    internal static class LoggerInstaller
    {
        public static IServiceCollection AddLogger(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetRequiredSection("Logger").Get<LoggerOptions>();
            var sinkOptions = new ElasticsearchSinkOptions(options.ElasticsearchOptions.Uri)
            {
                IndexFormat = "devup-logs-{0:yyyy-MM}",
                AutoRegisterTemplate = true,
                AutoRegisterTemplateVersion = AutoRegisterTemplateVersion.ESv7,
                TypeName = null
            };

            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .Enrich.WithMachineName()
                .Enrich.WithEnvironmentName()
                .WriteTo.Console()
                .WriteTo.Elasticsearch(sinkOptions)
                .ReadFrom.Configuration(configuration, sectionName: "Logger:Serilog")
                .Filter.ByExcluding(Matching.WithProperty<string>("RequestPath", rp => options.ExcludePaths.Any(ep => ep.EndsWith(rp))))
                .CreateLogger();

            services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));
            return services;
        }
    }
}

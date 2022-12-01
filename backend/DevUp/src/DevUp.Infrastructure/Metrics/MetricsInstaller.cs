using System.Linq;
using App.Metrics;
using App.Metrics.Formatters.Prometheus;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Infrastructure.Metrics
{
    internal static class MetricsInstaller
    {
        public static IServiceCollection AddMetrics(this IServiceCollection services, IConfiguration configuration)
        {
            var options = configuration.GetRequiredSection("Metrics").Get<Setup.MetricsOptions>();
            var metrics = AppMetrics.CreateDefaultBuilder()
                .OutputMetrics.AsPrometheusPlainText()
                .OutputMetrics.AsPrometheusProtobuf()
                .Configuration.Configure(opts =>
                {
                    opts.DefaultContextLabel = options.DefaultContextLabel;
                    opts.AddAppTag(options.AppTag);
                    opts.AddEnvTag(options.EnvTag);
                    opts.AddServerTag(options.ServerTag);
                }).Build();

            services.AddMetrics(metrics);
            services.AddMetricsTrackingMiddleware();
            services.AddMetricsEndpoints(opts =>
            {
                opts.MetricsTextEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusTextOutputFormatter>().First();
                opts.MetricsEndpointOutputFormatter = metrics.OutputMetricsFormatters.OfType<MetricsPrometheusProtobufOutputFormatter>().First();
            });
            services.AddMetricsReportingHostedService();
            services.AddAppMetricsCollectors();

            return services;
        }

        public static IApplicationBuilder UseMetrics(this IApplicationBuilder app)
        {
            app.UseMetricsAllMiddleware();
            app.UseMetricsAllEndpoints();

            return app;
        }
    }
}

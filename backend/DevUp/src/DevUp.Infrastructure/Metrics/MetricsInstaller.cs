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
            var metrics = new MetricsBuilder().Configuration.Configure(opts =>
            {
                opts.DefaultContextLabel = options.DefaultContextLabel;
            })
            .OutputMetrics.AsPrometheusPlainText()
            .Build();

            services.AddMetricsTrackingMiddleware();
            services.AddMetricsEndpoints();
            services.AddMetricsReportingHostedService();
            services.AddMetrics(metrics);
            return services;

        }

        public static IApplicationBuilder UseMetrics(this IApplicationBuilder app)
        {
            var formatter = new MetricsPrometheusTextOutputFormatter();
            app.UseMetricsTextEndpoint(formatter);
            app.UseMetricsEndpoint(formatter);
            app.UseMetricsAllMiddleware();
            return app;
        }
    }
}

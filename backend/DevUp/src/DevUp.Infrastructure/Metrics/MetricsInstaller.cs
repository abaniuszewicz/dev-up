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
            services.AddMetrics();
            services.AddMetricsEndpoints(opts =>
            {
                opts.MetricsTextEndpointOutputFormatter = new MetricsPrometheusTextOutputFormatter();
                opts.MetricsEndpointOutputFormatter = new MetricsPrometheusProtobufOutputFormatter();
            });

            return services;
        }

        public static IApplicationBuilder UseMetrics(this IApplicationBuilder app)
        {
            app.UseMetricsAllEndpoints();
            return app;
        }
    }
}

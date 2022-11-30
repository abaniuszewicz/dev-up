using App.Metrics;
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
            }).Build();

            services.AddMetricsTrackingMiddleware();
            services.AddMetricsEndpoints();
            services.AddMetricsReportingHostedService();
            services.AddMetrics(metrics);
            return services;

        }

        public static IApplicationBuilder UseMetrics(this IApplicationBuilder app)
        {
            app.UseMetricsAllEndpoints();
            app.UseMetricsAllMiddleware();
            return app;
        }
    }
}

using DevUp.Application.Identity;
using DevUp.Infrastructure.Documentation;
using DevUp.Infrastructure.Identity;
using DevUp.Infrastructure.Logging;
using DevUp.Infrastructure.Metrics;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevUp.Infrastructure
{
    public static class InfrastructureInstaller
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddLogger(configuration);
            services.AddHttpContextAccessor();
            services.AddScoped<ITokenStore, HttpContextTokenStore>();
            services.AddSwagger();
            services.AddMetrics(configuration);
            return services;
        }

        public static IApplicationBuilder UseHttpInfrastructure(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseSwaggerDoc();

            app.UseMetrics();
            return app;
        }
    }
}

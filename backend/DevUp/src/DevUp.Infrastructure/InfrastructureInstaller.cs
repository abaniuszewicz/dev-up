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
            services.AddLogger();
            services.AddHttpContextAccessor();
            services.AddScoped<ITokenStore, HttpContextTokenStore>();
            services.AddSwagger();
            services.AddMetrics(configuration);
            return services;
        }

        public static IApplicationBuilder UseHttpInfrastructure(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMetrics();
            if (env.IsDevelopment())
                app.UseSwaggerDoc();

            return app;
        }
    }
}

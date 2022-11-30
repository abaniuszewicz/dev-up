using DevUp.Application.Identity;
using DevUp.Infrastructure.Documentation;
using DevUp.Infrastructure.Identity;
using DevUp.Infrastructure.Logging;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevUp.Infrastructure
{
    public static class InfrastructureInstaller
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddLogger();
            services.AddHttpContextAccessor();
            services.AddScoped<ITokenStore, HttpContextTokenStore>();
            services.AddSwagger();
            return services;
        }

        public static IApplicationBuilder UseHttpInfrastructure(this IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
                app.UseSwaggerDoc();

            return app;
        }
    }
}

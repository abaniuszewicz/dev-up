using DevUp.Application.Identity;
using DevUp.Infrastructure.Http.Documentation;
using DevUp.Infrastructure.Http.Identity;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevUp.Infrastructure.Http
{
    public static class HttpInfrastructureInstaller
    {
        public static IServiceCollection AddHttpInfrastructure(this IServiceCollection services)
        {
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

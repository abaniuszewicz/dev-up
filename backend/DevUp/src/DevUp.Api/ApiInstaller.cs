using DevUp.Api.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Api
{
    public static class ApiInstaller
    {
        public static IServiceCollection AddApi(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAuthorization();
            services.AddRouting();
            services.AddAutoMapper(typeof(IApiMarker).Assembly);
            services.AddSingleton<ApplicationErrorHandler>();
            services.AddSingleton<InfrastructureErrorHandler>();
            services.AddSingleton<DomainErrorHandler>();
            return services;
        }

        public static IApplicationBuilder UseApi(this IApplicationBuilder app)
        {
            app.UseAuthorization();
            app.UseMiddleware<ApplicationErrorHandler>();
            app.UseMiddleware<InfrastructureErrorHandler>();
            app.UseMiddleware<DomainErrorHandler>();
            return app;
        }
    }
}

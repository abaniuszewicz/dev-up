using DevUp.Api.Configuration;
using DevUp.Api.V1.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Api
{
    internal static class ApiInstaller
    {
        public static IServiceCollection AddApi(this IServiceCollection services)
        {
            services.AddControllers(opt => opt.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())));
            services.AddEndpointsApiExplorer();
            services.AddRouting();
            services.AddAutoMapper(typeof(IApiMarker).Assembly);
            services.AddSingleton<ApplicationErrorHandler>();
            services.AddSingleton<InfrastructureErrorHandler>();
            services.AddSingleton<DomainErrorHandler>();
            return services;
        }

        public static IApplicationBuilder UseApi(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMiddleware<ApplicationErrorHandler>();
            app.UseMiddleware<InfrastructureErrorHandler>();
            app.UseMiddleware<DomainErrorHandler>();
            app.UseEndpoints(opt => opt.MapControllers());
            return app;
        }
    }
}

using DevUp.Api.Configuration;
using DevUp.Api.V1.Middlewares;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Api
{
    public static class ApiInstaller
    {
        public static IServiceCollection AddApi(this IServiceCollection services)
        {
            services.AddControllers(opt => opt.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer())));
            services.AddEndpointsApiExplorer();
            services.AddRouting();
            services.AddAutoMapper(typeof(IApiMarker).Assembly);
            services.AddSingleton<ValidationErrorHandler>();
            services.AddSingleton<InfrastructureErrorHandler>();
            return services;
        }

        public static IApplicationBuilder UseApi(this IApplicationBuilder app)
        {
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMiddleware<ValidationErrorHandler>();
            app.UseMiddleware<InfrastructureErrorHandler>();
            app.UseEndpoints(opt => opt.MapControllers());
            return app;
        }
    }
}

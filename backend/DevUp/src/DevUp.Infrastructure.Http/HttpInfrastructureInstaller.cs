using DevUp.Application.Identity;
using DevUp.Infrastructure.Http.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Infrastructure.Http
{
    public static class HttpInfrastructureInstaller
    {
        public static IServiceCollection AddHttpInfrastructure(this IServiceCollection services)
        {
            services.AddHttpContextAccessor();
            services.AddScoped<ITokenStore, HttpContextTokenStore>();
            return services;
        }
    }
}

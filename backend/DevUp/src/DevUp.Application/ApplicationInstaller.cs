using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Application
{
    public static class ApplicationInstaller
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(IApplicationMarker));
            return services;
        }
    }
}

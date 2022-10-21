using DevUp.Application.PipelineBehaviors;
using DevUp.Domain;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Application
{
    public static class ApplicationInstaller
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDomain(configuration);
            services.AddMediatR(typeof(IApplicationMarker));
            services.AddValidatorsFromAssemblyContaining<IApplicationMarker>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddAutoMapper(typeof(IApplicationMarker).Assembly);

            return services;
        }
    }
}

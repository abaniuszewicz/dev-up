using DevUp.Application.Organization.Commands;
using DevUp.Application.PipelineBehaviors;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Application
{
    public static class ApplicationInstaller
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(typeof(IApplicationMarker));
            services.AddValidatorsFromAssemblyContaining<IApplicationMarker>();
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            services.AddAutoMapper(typeof(IApplicationMarker).Assembly);

            return services;
        }
    }
}

using DevUp.Domain.Common.Services;
using DevUp.Domain.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Domain.Identity
{
    public static class IdentityInstaller
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<AuthenticationOptions>(configuration.GetRequiredSection("Authentication"));
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IPasswordService, PasswordService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IDateTimeProvider, UtcDateTimeProvider>();
            services.AddTransient(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
            return services;
        }
    }
}

using DevUp.Domain.Common.Services;
using DevUp.Domain.Identity.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Domain.Identity
{
    public static class IdentityInstaller
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IPasswordService, PasswordService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IDateTimeProvider, UtcDateTimeProvider>();
            services.AddTransient(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));
            return services;
        }
    }
}

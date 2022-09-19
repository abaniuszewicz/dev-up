using DevUp.Domain.Common.Services;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.Setup;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Domain.Identity
{
    public static class IdentityInstaller
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IPasswordService, PasswordService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IDateTimeProvider, UtcDateTimeProvider>();
            services.AddTransient(typeof(IPasswordHasher<>), typeof(PasswordHasher<>));

            services.Configure<AuthenticationOptions>(configuration.GetRequiredSection("Authentication"));
            services.ConfigureOptions<ConfigureJwtBearerOptions>();
            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer();

            return services;
        }
    }
}

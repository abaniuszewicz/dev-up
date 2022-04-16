using DevUp.Domain.Identity;
using DevUp.Infrastructure.Identity.Stores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DevUp.Infrastructure.Identity
{
    public static class JwtIdentityInstaller
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            var jwtSettings = new JwtSettings();
            services.AddSingleton(jwtSettings);
            services.AddScoped<IIdentityService, JwtIdentityService>();
            services.AddUserManager();

            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opts =>
            {
                opts.SaveToken = true;
                opts.TokenValidationParameters = new()
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(jwtSettings.Secret),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    RequireExpirationTime = false,
                    ValidateLifetime = true,
                };
            });

            return services;
        }

        private static IServiceCollection AddUserManager(this IServiceCollection services)
        {
            services.AddSingleton<IUserStore<User>, InMemoryUserStore>();
            services.AddSingleton<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddSingleton<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.AddScoped<IdentityErrorDescriber>();
            services.AddScoped<UserManager<User>>();
            return services;
        }
    }
}

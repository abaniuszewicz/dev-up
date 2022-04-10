using DevUp.Domain.Identity;
using DevUp.Infrastructure.Identity.Stores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DevUp.Infrastructure.Identity
{
    public static class IdentityInstaller
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            var jwtSettings = new JwtSettings();
            services.AddSingleton(jwtSettings);
            services.AddScoped<IIdentityService, IdentityService>();
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
            services.AddScoped<IUserStore<User>, UserStore>();
            services.AddScoped<IRoleStore<Role>, RoleStore>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
            services.AddScoped<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.AddScoped<IdentityErrorDescriber>();
            services.AddScoped<UserManager<User>>();
            return services;
        }
    }
}

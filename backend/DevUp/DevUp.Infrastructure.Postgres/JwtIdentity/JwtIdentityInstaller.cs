using DevUp.Domain.Identity;
using DevUp.Infrastructure.Postgres.JwtIdentity.Dtos;
using DevUp.Infrastructure.Postgres.JwtIdentity.Stores;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DevUp.Infrastructure.Postgres.JwtIdentity
{
    public static class JwtIdentityInstaller
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            var jwtSettings = new JwtSettings();
            services.AddSingleton(jwtSettings);
            services.AddScoped<IIdentityService, JwtIdentityService>();
            // services.AddUserManager();

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

        public static IServiceCollection AddPostgresUserManager(this IServiceCollection services)
        {
            services.AddSingleton<IUserStore<UserDto>, UserSTore>();
            services.AddSingleton<IPasswordHasher<UserDto>, PasswordHasher<UserDto>>();
            services.AddSingleton<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.AddScoped<IdentityErrorDescriber>();
            services.AddScoped<UserManager<UserDto>>();
            return services;
        }
    }
}

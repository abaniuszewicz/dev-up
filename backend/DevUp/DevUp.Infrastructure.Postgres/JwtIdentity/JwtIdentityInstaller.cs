using DevUp.Domain.Identity.Services;
using DevUp.Infrastructure.Postgres.JwtIdentity.Dtos;
using DevUp.Infrastructure.Postgres.JwtIdentity.Stores;
using DevUp.Infrastructure.Postgres.JwtIdentity.Validators;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Infrastructure.Postgres.JwtIdentity
{
    public static class JwtIdentityInstaller
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services)
        {
            services.AddTransient<IIdentityService, JwtIdentityService>();
            services.AddTransient<IRefreshTokenStore, RefreshTokenStore>();
            services.AddPostgresUserManager();


            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opts =>
            {
                opts.SaveToken = true;
                opts.TokenValidationParameters = tokenValidationParameters;
            });

            return services;
        }

        private static IServiceCollection AddPostgresUserManager(this IServiceCollection services)
        {
            services.AddTransient<IUserStore<UserDto>, UserStore>();
            services.AddSingleton<IPasswordHasher<UserDto>, PasswordHasher<UserDto>>();
            services.AddSingleton<ILookupNormalizer, UpperInvariantLookupNormalizer>();
            services.AddSingleton<IUserValidator<UserDto>, UsernameValidator>();
            services.AddSingleton<IUserValidator<UserDto>, UserValidator<UserDto>>();
            services.AddSingleton<IPasswordValidator<UserDto>, PasswordValidator<UserDto>>();
            services.AddSingleton<IdentityErrorDescriber>();
            services.AddTransient<UserManager<UserDto>>();
            return services;
        }
    }
}

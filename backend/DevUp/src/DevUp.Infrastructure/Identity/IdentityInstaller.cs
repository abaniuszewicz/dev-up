using DevUp.Domain.Common.Services;
using DevUp.Domain.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;

namespace DevUp.Infrastructure.Identity
{
    internal static class IdentityInstaller
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services, ISecretProvider secretProvider)
        {
            var jwtSettings = new JwtSettings(secretProvider);
            services.AddSingleton<IJwtSettings>(jwtSettings);

            services.AddAuthentication(opts =>
            {
                opts.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                opts.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(opts =>
            {
                opts.SaveToken = true;
                opts.TokenValidationParameters = jwtSettings.TokenValidationParameters;
            });

            return services;
        }
    }
}

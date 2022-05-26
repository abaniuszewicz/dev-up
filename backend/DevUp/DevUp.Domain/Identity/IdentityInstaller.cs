using DevUp.Common;
using DevUp.Domain.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace DevUp.Domain.Identity
{
    public static class IdentityInstaller
    {
        public static IServiceCollection AddIdentity(this IServiceCollection services)
        {
            var jwtSettings = new JwtSettings();
            services.AddSingleton(jwtSettings);

            services.AddTransient<IIdentityService, IdentityService>();
            services.AddTransient<IPasswordService, PasswordService>();
            services.AddTransient<ITokenService, TokenService>();
            services.AddTransient<IDateTimeProvider, DefaultDateTimeProvider>();

            var tokenValidationParameters = GetTokenValidationParameters(jwtSettings);
            services.AddSingleton(tokenValidationParameters);

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

        private static TokenValidationParameters GetTokenValidationParameters(JwtSettings jwtSettings)
        {
            return new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(jwtSettings.Secret),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
            };
        }
    }
}

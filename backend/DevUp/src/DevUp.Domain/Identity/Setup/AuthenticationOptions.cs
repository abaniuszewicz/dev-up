using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DevUp.Domain.Identity.Setup
{
    public sealed class AuthenticationOptions
    {
        public string SigningKey { get; set; }
        public string Algorithm { get; set; }
        public TimeSpan TokenExpiry { get; set; }
        public TimeSpan RefreshTokenExpiry { get; set; }

        public TokenValidationParameters GetTokenValidationParameters()
        {
            return new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = GetSecurityKey(),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
            };
        }

        public SigningCredentials GetSigningCredentials()
        {
            var securityKey = GetSecurityKey();
            return new SigningCredentials(securityKey, Algorithm);
        }

        private SecurityKey GetSecurityKey()
        {
            var bytes = Encoding.ASCII.GetBytes(SigningKey);
            return new SymmetricSecurityKey(bytes);
        }
    }
}

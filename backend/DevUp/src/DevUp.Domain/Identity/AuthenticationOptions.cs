using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace DevUp.Domain.Identity
{
    public sealed class AuthenticationOptions
    {
        public string SigningKey { get; set; }
        public TimeSpan TokenExpiry { get; set; }
        public TimeSpan RefreshTokenExpiry { get; set; }

        public TokenValidationParameters GetTokenValidationParameters()
        {
            var bytes = Encoding.ASCII.GetBytes(SigningKey);
            return new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(bytes),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
            };
        }
    }
}

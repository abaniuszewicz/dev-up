using System.Globalization;
using System.Text;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Identity;
using Microsoft.IdentityModel.Tokens;

namespace DevUp.Infrastructure.Identity
{
    internal class JwtSettings : IJwtSettings
    {
        public byte[] Secret { get; }
        public int JwtExpiryMs { get; }
        public int JwtRefreshExpiryMs { get; }
        public TokenValidationParameters TokenValidationParameters { get; }

        public JwtSettings(ISecretProvider secretProvider)
        {
            Secret = secretProvider.Get("JWT_SECRET", Encoding.ASCII.GetBytes);
            JwtExpiryMs = secretProvider.Get("JWT_EXPIRY_MS", s => int.Parse(s, NumberStyles.None, CultureInfo.InvariantCulture));
            JwtRefreshExpiryMs = secretProvider.Get("JWT_REFRESH_EXPIRY_MS", s => int.Parse(s, NumberStyles.None, CultureInfo.InvariantCulture));
            TokenValidationParameters = new TokenValidationParameters()
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Secret),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false,
                ValidateLifetime = true,
            };
        }
    }
}

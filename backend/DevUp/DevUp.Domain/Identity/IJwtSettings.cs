using Microsoft.IdentityModel.Tokens;

namespace DevUp.Domain.Identity
{
    public interface IJwtSettings
    {
        public byte[] Secret { get; }
        public int JwtExpiryMs { get; }
        public int JwtRefreshExpiryMs { get; }
        public TokenValidationParameters TokenValidationParameters { get; }
    }
}

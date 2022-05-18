using System.Globalization;
using System.Text;

namespace DevUp.Infrastructure.Postgres.JwtIdentity
{
    internal class JwtSettings
    {
        public byte[] Secret { get; }
        public int JwtExpiryMs { get; }
        public int JwtRefreshExpiryMs { get; }

        public JwtSettings()
        {
            var secret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new InvalidOperationException("Failed to find JWT_SECRET definition");
            var jwtExpiry = Environment.GetEnvironmentVariable("JWT_EXPIRY_MS")
                ?? throw new InvalidOperationException("Failed to find JWT_EXPIRY_MS definition");
            var jwtRefreshExpiry = Environment.GetEnvironmentVariable("JWT_REFRESH_EXPIRY_MS")
                ?? throw new InvalidOperationException("Failed to find JWT_REFRESH_EXPIRY_MS definition");

            Secret = Encoding.ASCII.GetBytes(secret);
            JwtExpiryMs = int.TryParse(jwtExpiry, NumberStyles.Integer, CultureInfo.InvariantCulture, out var jwtExpiryMs) && jwtExpiryMs > 0
                ? jwtExpiryMs
                : throw new InvalidOperationException("Invalid value for JWT_EXPIRY_MS. Value has to be positive integer.");
            JwtRefreshExpiryMs = int.TryParse(jwtRefreshExpiry, NumberStyles.Integer, CultureInfo.InvariantCulture, out var jwtRefreshExpiryMs) && jwtRefreshExpiryMs > 0
                ? jwtRefreshExpiryMs
                : throw new InvalidOperationException("Invalid value for JWT_REFRESH_EXPIRY_MS. Value has to be positive integer.");
        }
    }
}

using System.Globalization;
using System.Text;

namespace DevUp.Infrastructure.Postgres.JwtIdentity
{
    internal class JwtSettings
    {
        public byte[] Secret { get; }
        public int ExpiryMs { get; }

        public JwtSettings()
        {
            var secret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new InvalidOperationException("Failed to find JWT_SECRET definition");
            var expiry = Environment.GetEnvironmentVariable("JWT_EXPIRY_MS")
                ?? throw new InvalidOperationException("Failed to find JWT_EXPIRY_MS definition");

            Secret = Encoding.ASCII.GetBytes(secret);
            ExpiryMs = int.TryParse(expiry, NumberStyles.Integer, CultureInfo.InvariantCulture, out var expiryMs) && expiryMs > 0
                ? expiryMs
                : throw new InvalidOperationException("Invalid value for JWT_EXPIRY_MS. Value has to be positive integer.");
        }
    }
}

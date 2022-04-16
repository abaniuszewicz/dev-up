using System.Text;

namespace DevUp.Infrastructure.Postgres.JwtIdentity
{
    internal class JwtSettings
    {
        public byte[] Secret { get; }

        public JwtSettings()
        {
            var secret = Environment.GetEnvironmentVariable("JWT_SECRET")
                ?? throw new InvalidOperationException("Failed to find JWT_SECRET definition");

            Secret = Encoding.ASCII.GetBytes(secret);
        }
    }
}

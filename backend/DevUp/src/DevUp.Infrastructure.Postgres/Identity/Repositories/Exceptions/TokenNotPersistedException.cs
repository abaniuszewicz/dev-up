using DevUp.Infrastructure.Exceptions;
using DevUp.Infrastructure.Postgres.Identity.Dtos;

namespace DevUp.Infrastructure.Postgres.Identity.Repositories.Exceptions
{
    internal sealed class TokenNotPersistedException : InfrastructureException
    {
        public RefreshTokenDto RefreshToken { get; }

        public TokenNotPersistedException(RefreshTokenDto refreshToken)
            : base("Failed to persist refresh token.")
        {
            RefreshToken = refreshToken;
        }
    }
}

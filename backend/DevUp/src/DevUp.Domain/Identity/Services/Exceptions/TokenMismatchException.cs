using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class TokenMismatchException : IdentityException
    {
        public string JtiFromToken { get; }
        public string JtiFromRefreshToken { get; }

        public TokenMismatchException(TokenInfo token, RefreshTokenInfo refreshToken)
            : base("Mismatch on jti from token and refresh token.")
        {
            JtiFromToken = token.Jti;
            JtiFromRefreshToken = refreshToken.Jti;
        }
    }
}

using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class RefreshTokenUsedException : IdentityBusinessRuleValidationException
    {
        public RefreshToken RefreshToken { get; }

        public RefreshTokenUsedException(RefreshToken refreshToken)
            : base("Refresh token has been already used.")
        {
            RefreshToken = refreshToken;
        }
    }
}

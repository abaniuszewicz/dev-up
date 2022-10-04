using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class RefreshTokenInvalidatedException : IdentityBusinessRuleValidationException
    {
        public RefreshToken RefreshToken { get; }

        public RefreshTokenInvalidatedException(RefreshToken refreshToken)
            : base("Refresh token has been invalidated.")
        {
            RefreshToken = refreshToken;
        }
    }
}

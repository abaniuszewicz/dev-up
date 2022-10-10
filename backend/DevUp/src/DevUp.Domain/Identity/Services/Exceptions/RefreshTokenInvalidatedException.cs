using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class RefreshTokenInvalidatedException : IdentityBusinessRuleValidationException
    {
        public RefreshTokenInfoId RefreshToken { get; }

        public RefreshTokenInvalidatedException(RefreshTokenInfoId refreshToken)
            : base("Refresh token has been invalidated.")
        {
            RefreshToken = refreshToken;
        }
    }
}

using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class RefreshTokenInvalidatedException : IdentityDataValidationException
    {
        public RefreshTokenInvalidatedException()
            : base("Refresh token has been invalidated.")
        {
        }
    }
}

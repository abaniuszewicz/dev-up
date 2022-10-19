using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects.Exceptions
{
    internal sealed class EmptyRefreshTokenException : IdentityDataValidationException
    {
        public EmptyRefreshTokenException()
            : base("Refresh token cannot be empty.")
        {
        }
    }
}

using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects.Exceptions
{
    internal sealed class InvalidTokenFormatException : IdentityDataValidationException
    {
        public InvalidTokenFormatException()
            : base("Token is not a valid jwt token.")
        {
        }
    }
}

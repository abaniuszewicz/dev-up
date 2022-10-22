using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects.Exceptions
{
    internal sealed class EmptyTokenException : IdentityDataValidationException
    {
        public EmptyTokenException()
            : base("Token cannot be empty.")
        {
        }
    }
}

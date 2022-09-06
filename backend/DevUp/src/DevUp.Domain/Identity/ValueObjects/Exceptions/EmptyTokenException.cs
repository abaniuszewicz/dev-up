using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects.Exceptions
{
    internal sealed class EmptyTokenException : IdentityValidationException
    {
        public EmptyTokenException()
            : base("Token cannot be empty.")
        {
        }
    }
}

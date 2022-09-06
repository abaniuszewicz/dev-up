using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects.Exceptions
{
    internal sealed class EmptyPasswordException : IdentityValidationException
    {
        public EmptyPasswordException()
            : base("Password cannot be empty.")
        {
        }
    }
}

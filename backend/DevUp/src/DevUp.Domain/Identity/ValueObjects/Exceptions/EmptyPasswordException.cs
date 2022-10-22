using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects.Exceptions
{
    internal sealed class EmptyPasswordException : IdentityDataValidationException
    {
        public EmptyPasswordException()
            : base("Password cannot be empty.")
        {
        }
    }
}

using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects.Exceptions
{
    internal sealed class EmptyUsernameException : IdentityValidationException
    {
        public EmptyUsernameException()
            : base("Username cannot be empty.")
        {
        }
    }
}

using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects.Exceptions
{
    internal sealed class EmptyPasswordException : ValidationException
    {
        public EmptyPasswordException()
            : base("Password cannot be empty.")
        {
        }
    }
}

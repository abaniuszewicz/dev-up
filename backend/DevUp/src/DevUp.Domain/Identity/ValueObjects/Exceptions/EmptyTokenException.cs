using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects.Exceptions
{
    internal sealed class EmptyTokenException : ValidationException
    {
        public EmptyTokenException()
            : base("Token cannot be empty.")
        {
        }
    }
}

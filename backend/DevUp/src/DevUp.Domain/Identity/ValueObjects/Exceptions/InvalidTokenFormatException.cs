using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects.Exceptions
{
    internal sealed class InvalidTokenFormatException : ValidationException
    {
        public InvalidTokenFormatException()
            : base("Token is not a valid jwt token.")
        {
        }
    }
}

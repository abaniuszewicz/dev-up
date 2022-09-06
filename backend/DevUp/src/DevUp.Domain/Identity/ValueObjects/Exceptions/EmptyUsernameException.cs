using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects.Exceptions
{
    public class EmptyUsernameException : ValidationException
    {
        public EmptyUsernameException()
            : base("Username cannot be empty.")
        {
        }
    }
}

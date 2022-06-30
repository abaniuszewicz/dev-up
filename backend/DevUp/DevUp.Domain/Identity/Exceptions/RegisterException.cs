using System.Collections.Generic;

namespace DevUp.Domain.Identity.Exceptions
{
    public class RegisterException : IdentityException
    {
        internal const string UsernameTakenMessage = "User with this username already exist.";
        internal const string CreationFailedMessage = "Failed to persist the user.";

        public RegisterException(string error) : base(error)
        {
        }

        public RegisterException(IEnumerable<string> errors) : base(errors)
        {
        }
    }
}

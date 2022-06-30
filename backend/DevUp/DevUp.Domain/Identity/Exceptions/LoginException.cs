using System.Collections.Generic;

namespace DevUp.Domain.Identity.Exceptions
{
    public class LoginException : IdentityException
    {
        internal const string InvalidUsernameMessage = "User with this username does not exists.";
        internal const string InvalidPasswordMessage = "Invalid password.";
        internal const string HashNotFoundMessage = "Failed to retrieve password hash.";

        public LoginException(string error) : base(error)
        {
        }

        public LoginException(IEnumerable<string> errors) : base(errors)
        {
        }
    }
}

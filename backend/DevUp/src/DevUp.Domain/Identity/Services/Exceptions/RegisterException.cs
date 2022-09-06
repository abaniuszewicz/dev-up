using System.Collections.Generic;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    internal sealed class RegisterException : IdentityValidationException
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

using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.Exceptions
{
    public class UsernameValidationException : ValidationException
    {
        internal const string NullMessage = "Username cannot be null.";
        internal const string InvalidLengthMessage = "Username must be 6-30 characters long.";
        internal const string InvalidCharactersMessage = "Username may only contain lowercase letters or hyphens.";
        internal const string InvalidFirstOrLastCharacterMessage = "Username cannot begin or end with a hyphen.";

        public UsernameValidationException(string error) : base(error)
        {
        }

        public UsernameValidationException(IEnumerable<string> errors) : base(errors)
        {
        }
    }
}

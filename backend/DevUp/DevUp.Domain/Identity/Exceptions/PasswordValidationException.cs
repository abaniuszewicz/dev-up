using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.Exceptions
{
    public class PasswordValidationException : ValidationException
    {
        internal const string NullMessage = "Password cannot be null.";
        internal const string TooShortMessage = "Password must be at least 8 characters long.";
        internal const string NoLowercaseLetterMessage = "Password must contain at least one lower case letter.";
        internal const string NoUppercaseLetterMessage = "Password must contain at least one upper case letter.";
        internal const string NoSpecialCharacterMessage = "Password must contain at least one special character.";
        internal const string NoDigitMessage = "Password must contain at least one digit.";

        public PasswordValidationException(string error) 
            : base(error)
        {
        }

        public PasswordValidationException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

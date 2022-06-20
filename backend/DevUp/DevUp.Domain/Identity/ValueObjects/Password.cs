using System.Collections.Generic;
using System.Linq;
using DevUp.Common.Extensions;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Seedwork;

using static DevUp.Domain.Identity.Exceptions.PasswordValidationException;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class Password : ValueObject
    {
        public string Value { get; }

        public Password(string password)
        {
            Validate(password);
            Value = password;
        }

        public override string ToString()
        {
            return Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        private static void Validate(string password)
        {
            if (password is null)
                throw new PasswordValidationException(NullMessage);

            var errors = new List<string>();
            if (password.Length < 8)
                errors.Add(TooShortMessage);
            if (password.None(char.IsLower))
                errors.Add(NoLowercaseLetterMessage);
            if (password.None(char.IsUpper))
                errors.Add(NoUppercaseLetterMessage);
            if (password.None(c => !char.IsLetterOrDigit(c)))
                errors.Add(NoSpecialCharacterMessage);
            if (password.None(char.IsDigit))
                errors.Add(NoDigitMessage);

            if (errors.Any())
                throw new PasswordValidationException(errors);
        }
    }
}

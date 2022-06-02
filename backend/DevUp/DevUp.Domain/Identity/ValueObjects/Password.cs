using System.Collections.Generic;
using System.Linq;
using DevUp.Common.Extensions;
using DevUp.Domain.Seedwork;
using DevUp.Domain.Seedwork.Exceptions;

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
                throw new ValidationException("Password cannot be null.");

            var errors = new List<string>();
            if (password.Length < 8)
                errors.Add("Password must be at least 8 characters long.");
            if (password.None(char.IsLower))
                errors.Add("Password must contain at least one lower case letter.");
            if (password.None(char.IsUpper))
                errors.Add("Password must contain at least one upper case letter.");
            if (password.None(char.IsLetterOrDigit))
                errors.Add("Password must contain at least one special character.");
            if (password.None(char.IsDigit))
                errors.Add("Password must contain at least one digit.");

            if (errors.Any())
                throw new ValidationException(errors);
        }
    }
}

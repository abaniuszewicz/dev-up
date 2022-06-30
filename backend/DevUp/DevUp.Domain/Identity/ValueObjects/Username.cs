using System.Collections.Generic;
using System.Linq;
using DevUp.Domain.Seedwork;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class Username : ValueObject
    {
        public string Value { get; set; }

        public Username(string username)
        {
            Validate(username);
            Value = username;
        }

        public override string ToString()
        {
            return Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        private static void Validate(string username)
        {
            if (username is null)
                throw new ValidationException("Username cannot be null.");

            var errors = new List<string>();
            if (username.Length < 6 || username.Length > 30)
                errors.Add("Username must be 6-30 characters long.");
            if (username.Any(c => !IsValidCharacter(c)))
                errors.Add("Username may only contain alphanumeric characters or hyphens.");
            if (username.StartsWith('-') || username.EndsWith('-'))
                errors.Add("Username cannot begin or end with a hyphen.");

            if (errors.Any())
                throw new ValidationException(errors);
        }

        private static bool IsValidCharacter(char c)
        {
            return c switch
            {
                >= 'a' and <= 'z' => true,
                '-' => true,
                _ => false
            };
        }
    }
}

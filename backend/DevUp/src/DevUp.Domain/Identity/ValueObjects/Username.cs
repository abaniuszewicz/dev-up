using System.Collections.Generic;
using System.Linq;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Seedwork;

using static DevUp.Domain.Identity.Exceptions.UsernameValidationException;

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
                throw new UsernameValidationException(NullMessage);

            var errors = new List<string>();
            if (username.Length < 6 || username.Length > 30)
                errors.Add(InvalidLengthMessage);
            if (username.Any(c => !IsValidCharacter(c)))
                errors.Add(InvalidCharactersMessage);
            if (username.StartsWith('-') || username.EndsWith('-'))
                errors.Add(InvalidFirstOrLastCharacterMessage);

            if (errors.Any())
                throw new UsernameValidationException(errors);
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

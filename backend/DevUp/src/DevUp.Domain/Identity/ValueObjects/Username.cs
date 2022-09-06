using System.Collections.Generic;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using DevUp.Domain.Seedwork;

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
            if (string.IsNullOrWhiteSpace(username))
                throw new EmptyUsernameException();
        }
    }
}

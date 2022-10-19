using System.Collections.Generic;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public sealed class Password : ValueObject
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
            if (string.IsNullOrEmpty(password))
                throw new EmptyPasswordException();
        }
    }
}

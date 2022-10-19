using System.Collections.Generic;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public sealed class PasswordHash : ValueObject
    {
        public string Value { get; }

        public PasswordHash(string passwordHash)
        {
            Value = passwordHash;
        }

        public override string ToString()
        {
            return Value;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}

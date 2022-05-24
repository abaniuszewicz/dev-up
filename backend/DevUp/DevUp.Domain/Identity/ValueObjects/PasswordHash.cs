using System.Collections.Generic;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class PasswordHash : ValueObject
    {
        public string Value { get; }

        public PasswordHash(string passwordHash)
        {
            Value = passwordHash;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}

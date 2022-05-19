using System.Collections.Generic;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class Token : ValueObject
    {
        public string Value { get; }

        public Token(string token)
        {
            Value = token;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}

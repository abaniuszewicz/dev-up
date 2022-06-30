using System.Collections.Generic;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Seedwork;

using static DevUp.Domain.Identity.Exceptions.TokenValidationException;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class Token : ValueObject
    {
        public string Value { get; }

        public Token(string token)
        {
            Validate(token);
            Value = token;
        }

        public override string ToString()
        {
            return Value;
        }

        private static void Validate(string token)
        {
            if (token is null)
                throw new TokenValidationException(TokenNullMessage);
            if (string.IsNullOrWhiteSpace(token))
                throw new TokenValidationException(TokenEmptyMessage);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}

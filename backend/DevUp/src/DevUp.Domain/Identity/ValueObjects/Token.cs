using System;
using System.Collections.Generic;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public sealed class Token : ValueObject
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
            if (string.IsNullOrWhiteSpace(token))
                throw new EmptyTokenException();

            var segments = token.Split('.', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
            if (segments.Length != 3)
                throw new InvalidTokenFormatException();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }
    }
}

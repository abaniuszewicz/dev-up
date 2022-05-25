using System.Collections.Generic;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Seedwork;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class Token : ValueObject
    {
        public string Value { get; }
        public string Jti { get; }
        public UserId UserId { get; }

        public Token()
        {
        }

        public Token(string token)
        {
            Validate(token);
            Value = token;
        }

        private static void Validate(string token)
        {
            if (token is null)
                throw new ValidationException("Token cannot be null.");
            if (string.IsNullOrWhiteSpace(token))
                throw new ValidationException("Token cannot be empty.");
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Jti;
        }
    }
}

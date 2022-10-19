using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class RefreshToken : ValueObject
    {
        public string Value { get; }

        internal RefreshToken()
            : this(GetRandomString())
        {
        }

        public RefreshToken(string token)
        {
            Validate(token);
            Value = token;
        }

        private void Validate(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
                throw new EmptyRefreshTokenException();
        }

        private static string GetRandomString()
        {
            var randomNumber = new byte[64];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
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

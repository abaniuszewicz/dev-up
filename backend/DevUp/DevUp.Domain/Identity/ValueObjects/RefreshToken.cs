using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class RefreshToken : ValueObject
    {
        public string Value { get; }

        public RefreshToken()
            : this(GetRandomToken())
        {
        }

        public RefreshToken(string token)
        {
            Value = token;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Value;
        }

        private static string GetRandomToken()
        {
            var randomNumber = new byte[64];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}

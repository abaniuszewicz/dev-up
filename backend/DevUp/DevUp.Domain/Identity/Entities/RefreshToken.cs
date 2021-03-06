using System;
using System.Security.Cryptography;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Entities
{
    public class RefreshToken : EntityId
    {
        public string Value { get; }

        internal RefreshToken() : this(GetRandomString())
        {
        }

        public RefreshToken(string token)
        {
            Value = token ?? throw new ArgumentNullException(nameof(token));
        }

        public override string ToString()
        {
            return Value;
        }

        public override bool Equals(EntityId other)
        {
            return other is RefreshToken refreshTokenId && Value == refreshTokenId.Value;
        }

        public override int GetHashCode()
        {
            return Value.GetHashCode();
        }

        private static string GetRandomString()
        {
            var randomNumber = new byte[64];
            using var generator = RandomNumberGenerator.Create();
            generator.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }
    }
}

using System;
using System.Security.Cryptography;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Entities
{
    public class RefreshTokenId : EntityId
    {
        public string Token { get; }

        public RefreshTokenId() : this(GetRandomToken())
        {

        }

        public RefreshTokenId(string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }

        public override bool Equals(EntityId other)
        {
            return other is RefreshTokenId refreshTokenId && Token == refreshTokenId.Token;
        }

        public override int GetHashCode()
        {
            return Token.GetHashCode();
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

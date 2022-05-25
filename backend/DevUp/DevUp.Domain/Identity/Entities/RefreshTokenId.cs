using System;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Entities
{
    public class RefreshTokenId : EntityId
    {
        public string Token { get; }

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
    }
}

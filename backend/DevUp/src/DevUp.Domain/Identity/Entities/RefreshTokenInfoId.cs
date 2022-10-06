using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Entities
{
    public class RefreshTokenInfoId : EntityId
    {
        public RefreshToken RefreshToken { get; }


        public RefreshTokenInfoId(RefreshToken refreshToken)
        {
            RefreshToken = refreshToken;
        }

        public override string ToString()
        {
            return RefreshToken.ToString();
        }

        public override bool Equals(EntityId other)
        {
            return other is RefreshTokenInfoId otherRefreshTokenId && RefreshToken == otherRefreshTokenId.RefreshToken;
        }

        public override int GetHashCode()
        {
            return RefreshToken.GetHashCode();
        }
    }
}

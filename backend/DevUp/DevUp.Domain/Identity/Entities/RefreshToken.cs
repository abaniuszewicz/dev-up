using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class RefreshToken : Entity<RefreshTokenId>
    {
        public bool Exists { get; }
        public bool Invalidated { get; }

        public RefreshToken(RefreshTokenId id) : base(id)
        {
        }
    }
}

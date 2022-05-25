using DevUp.Common;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class RefreshToken : Entity<RefreshTokenId>
    {
        public string Jti { get; }
        public UserId UserId { get; }
        public DeviceId DeviceId { get; }
        public DateTimeRange Lifespan { get; }
        public bool Used { get; set; }
        public bool Invalidated { get; set; }

        public RefreshToken(RefreshTokenId id, Token token, UserId userId, DeviceId deviceId, DateTimeRange lifespan) : base(id)
        {
            Jti = token.Jti;
            UserId = userId;
            DeviceId = deviceId;
            Lifespan = lifespan;
        }

        public bool BelongsTo(User user)
        {
            return UserId == user.Id;
        }

        public bool BelongsTo(Token token)
        {
            return Jti == token.Jti;
        }

        public bool IsActive(IDateTimeProvider dateTimeProvider)
        {
            return Lifespan.IsWithinRange(dateTimeProvider.UtcNow);
        }
    }
}

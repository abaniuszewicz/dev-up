using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Entities
{
    public sealed class RefreshTokenInfo : Entity<RefreshTokenInfoId>
    {
        public string Jti { get; }
        public UserId UserId { get; }
        public DeviceId DeviceId { get; }
        public DateTimeRange Lifespan { get; }
        public bool Used { get; set; }
        public bool Invalidated { get; set; }

        public RefreshTokenInfo(RefreshTokenInfoId token, string jti, UserId userId, DeviceId deviceId, DateTimeRange lifespan) : base(token)
        {
            Jti = jti;
            UserId = userId;
            DeviceId = deviceId;
            Lifespan = lifespan;
        }

        public bool BelongsTo(User user)
        {
            return UserId == user.Id;
        }

        public bool BelongsTo(TokenInfo token)
        {
            return Jti == token.Jti;
        }

        public bool BelongsTo(Device device)
        {
            return DeviceId == device.Id;
        }

        public bool IsActive(IDateTimeProvider dateTimeProvider)
        {
            return Lifespan.IsWithinRange(dateTimeProvider.Now);
        }
    }
}

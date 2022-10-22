using System.Collections.Generic;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public sealed class TokenInfo : ValueObject
    {
        public Token Token { get; }
        public string Jti { get; }
        public UserId UserId { get; }
        public DeviceId DeviceId { get; }
        public DateTimeRange Lifespan { get; }

        public TokenInfo(Token token, string jti, UserId userId, DeviceId deviceId, DateTimeRange lifespan)
        {
            Token = token;
            Jti = jti;
            UserId = userId;
            DeviceId = deviceId;
            Lifespan = lifespan;
        }

        public bool BelongsTo(User user)
        {
            return UserId == user.Id;
        }

        public bool BelongsTo(Device device)
        {
            return DeviceId == device.Id;
        }

        public bool IsActive(IDateTimeProvider dateTimeProvider)
        {
            return Lifespan.IsWithinRange(dateTimeProvider.Now);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Token;
        }
    }
}

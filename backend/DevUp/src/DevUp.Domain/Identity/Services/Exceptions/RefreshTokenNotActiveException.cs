using System;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class RefreshTokenNotActiveException : IdentityBusinessRuleValidationException
    {
        public RefreshToken RefreshToken { get; }
        public DateTimeRange Lifespan { get; }
        public DateTime Now { get; }

        public RefreshTokenNotActiveException(RefreshTokenInfo refreshTokenInfo, IDateTimeProvider dateTimeProvider)
            : base("Refresh token is not active.")
        {
            RefreshToken = refreshTokenInfo.Id;
            Lifespan = refreshTokenInfo.Lifespan;
            Now = dateTimeProvider.Now;
        }
    }
}

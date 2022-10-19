using System;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class RefreshTokenExpiredException : IdentityBusinessRuleValidationException
    {
        public RefreshTokenInfoId RefreshToken { get; }
        public DateTimeRange Lifespan { get; }
        public DateTime Now { get; }

        public RefreshTokenExpiredException(RefreshTokenInfo refreshTokenInfo, IDateTimeProvider dateTimeProvider)
            : base("Refresh token has expired.")
        {
            RefreshToken = refreshTokenInfo.Id;
            Lifespan = refreshTokenInfo.Lifespan;
            Now = dateTimeProvider.Now;
        }
    }
}

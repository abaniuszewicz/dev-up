using System;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class RefreshTokenNotActiveException : IdentityDataValidationException
    {
        public DateTimeRange Lifespan { get; }
        public DateTime Now { get; }

        public RefreshTokenNotActiveException(DateTimeRange lifespan, IDateTimeProvider dateTimeProvider)
            : base("Refresh token is not active.")
        {
            Lifespan = lifespan;
            Now = dateTimeProvider.Now;
        }
    }
}

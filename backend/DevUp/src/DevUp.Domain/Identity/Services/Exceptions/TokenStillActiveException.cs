using System;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class TokenStillActiveException : IdentityValidationException
    {
        public DateTimeRange Lifespan { get; }
        public DateTime Now { get; }

        public TokenStillActiveException(DateTimeRange lifespan, IDateTimeProvider dateTimeProvider)
            : base("Token is still active.")
        {
            Lifespan = lifespan;
            Now = dateTimeProvider.Now;
        }
    }
}

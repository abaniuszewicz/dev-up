using System;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class TokenNotExpiredException : IdentityBusinessRuleValidationException
    {
        public Token Token { get; }
        public DateTimeRange Lifespan { get; }
        public DateTime Now { get; }

        public TokenNotExpiredException(TokenInfo tokenInfo, IDateTimeProvider dateTimeProvider)
            : base("Token has not expired.")
        {
            Token = tokenInfo.Token;
            Lifespan = tokenInfo.Lifespan;
            Now = dateTimeProvider.Now;
        }
    }
}

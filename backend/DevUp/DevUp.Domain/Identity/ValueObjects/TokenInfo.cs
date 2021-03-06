using System.Collections.Generic;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class TokenInfo : ValueObject
    {
        public Token Token { get; }
        public string Jti { get; }
        public UserId UserId { get; }
        public DateTimeRange Lifespan { get; }

        public TokenInfo(Token token, string jti, UserId userId, DateTimeRange lifespan)
        {
            Token = token;
            Jti = jti;
            UserId = userId;
            Lifespan = lifespan;
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

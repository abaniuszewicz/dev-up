using System;
using DevUp.Common;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class RefreshToken : Entity<RefreshTokenId>
    {
        public string Jti { get; init; }
        public UserId UserId { get; init; }
        public DateTimeRange Lifespan { get; init; }
        public bool Used { get; init; }
        public bool Invalidated { get; init; }

        public RefreshToken(RefreshTokenId id) : base(id)
        {
        }
    }
}

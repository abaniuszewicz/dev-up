using System;
using DevUp.Domain.Entities;

namespace DevUp.Domain.Identity.Entities
{
    public class UserId : EntityId
    {
        public Guid Id { get; }

        public override bool Equals(EntityId other)
        {
            return other is UserId otherUserId && otherUserId.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

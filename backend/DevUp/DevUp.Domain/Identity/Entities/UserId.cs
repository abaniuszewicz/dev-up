using System;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Entities
{
    public class UserId : EntityId
    {
        public Guid Id { get; }

        public UserId() : this(Guid.NewGuid())
        {
        }

        public UserId(Guid id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }

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

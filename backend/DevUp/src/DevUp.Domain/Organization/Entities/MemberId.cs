using System;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Organization.Entities
{
    public sealed class MemberId : EntityId
    {
        public Guid Id { get; }

        public MemberId() : this(Guid.NewGuid())
        {
        }

        public MemberId(Guid id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public override bool Equals(EntityId other)
        {
            return other is MemberId otherMemberId && otherMemberId.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

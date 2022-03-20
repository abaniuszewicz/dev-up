using System;

namespace DevUp.Domain.Entities
{
    public abstract class EntityId<TId> : IEquatable<EntityId<TId>>
    {
        public TId Id { get; }

        protected EntityId(TId id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
        }

        public virtual bool Equals(EntityId<TId> other)
        {
            return other is not null && Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as EntityId<TId>);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

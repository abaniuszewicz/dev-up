using System;

namespace DevUp.Domain.Entities
{
    public abstract class Entity<TId> : IEquatable<Entity<TId>>
    {
        public EntityId<TId> Id { get; }
        public Entity(EntityId<TId> id)
        {
            Id = id;
        }

        public bool Equals(Entity<TId> other)
        {
            return Equals(Id, other.Id);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as Entity<TId>);
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

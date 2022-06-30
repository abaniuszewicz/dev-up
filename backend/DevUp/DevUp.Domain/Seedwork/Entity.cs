using System;

namespace DevUp.Domain.Seedwork
{
    public abstract class Entity<TId> : IEquatable<Entity<TId>>
        where TId : EntityId
    {
        public TId Id { get; }

        public Entity(TId id)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
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

        public static bool operator ==(Entity<TId> left, Entity<TId> right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(Entity<TId> left, Entity<TId> right)
        {
            return !(left == right);
        }
    }
}

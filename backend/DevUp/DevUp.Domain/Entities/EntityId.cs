using System;

namespace DevUp.Domain.Entities
{
    public abstract class EntityId : IEquatable<EntityId>
    {
        public abstract bool Equals(EntityId other);
        public abstract override int GetHashCode();

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) 
                return false;

            if (ReferenceEquals(this, obj)) 
                return true;

            if (obj.GetType() != GetType()) 
                return false;

            return Equals((EntityId)obj);
        }

        public static bool operator ==(EntityId left, object right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(EntityId left, object right)
        {
            return !(left == right);
        }
    }
}

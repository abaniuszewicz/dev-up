using System;
using System.Collections.Generic;
using System.Linq;

namespace DevUp.Domain.Seedwork
{
    public abstract class ValueObject : IEquatable<ValueObject>
    {
        protected abstract IEnumerable<object> GetEqualityComponents();

        public virtual bool Equals(ValueObject other)
        {
            if (other is null)
                return false;

            var otherEqualityComponents = other.GetEqualityComponents();
            return GetEqualityComponents().SequenceEqual(otherEqualityComponents);
        }

        public override bool Equals(object other)
        {
            if (other is null)
                return false;
            if (other.GetType() != GetType())
                return false;

            return Equals((ValueObject)other);
        }

        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(ec => ec is null ? 0 : ec.GetHashCode())
                .Aggregate((x, y) => x ^ y);
        }

        public static bool operator ==(ValueObject left, ValueObject right)
        {
            if (left is null)
                return right is null;

            return left.Equals(right);
        }

        public static bool operator !=(ValueObject left, ValueObject right)
        {
            return !(left == right);
        }
    }
}

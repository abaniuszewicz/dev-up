using System;

namespace DevUp.Domain.Entities.Identity
{
    public class UserId : EntityId
    {
        public UserId()
        {
        }

        public override bool Equals(EntityId other)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }
    }
}

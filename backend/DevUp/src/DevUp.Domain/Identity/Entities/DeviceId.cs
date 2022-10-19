using System;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Entities
{
    public sealed class DeviceId : EntityId
    {
        public Guid Id { get; }

        public DeviceId(Guid id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public override bool Equals(EntityId other)
        {
            return other is DeviceId deviceId && Id == deviceId.Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

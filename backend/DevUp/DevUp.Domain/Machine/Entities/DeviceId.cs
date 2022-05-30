using DevUp.Domain.Entities;

namespace DevUp.Domain.Machine.Entities
{
    public class DeviceId : EntityId
    {
        public string Id { get; }

        public DeviceId(string id)
        {
            Id = id;
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

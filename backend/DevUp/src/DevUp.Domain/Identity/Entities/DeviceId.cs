using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Entities
{
    public class DeviceId : EntityId
    {
        public string Id { get; }

        public DeviceId(string id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Id;
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

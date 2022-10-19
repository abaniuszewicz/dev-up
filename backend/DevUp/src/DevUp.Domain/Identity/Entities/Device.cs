using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Entities
{
    public class Device : Entity<DeviceId>
    {
        public DeviceName Name { get; }

        public Device(DeviceId id, DeviceName name)
            : base(id)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}

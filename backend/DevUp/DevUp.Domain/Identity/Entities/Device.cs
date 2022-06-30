using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Entities
{
    public class Device : Entity<DeviceId>
    {
        public string Name { get; }

        public Device(DeviceId id, string name)
            : base(id)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

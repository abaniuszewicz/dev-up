using DevUp.Domain.Entities;

namespace DevUp.Domain.Machine.Entities
{
    public class Device : Entity<DeviceId>
    {
        public string Name { get; }

        public Device(DeviceId id, string name) 
            : base(id)
        {
            Name = name;
        }
    }
}

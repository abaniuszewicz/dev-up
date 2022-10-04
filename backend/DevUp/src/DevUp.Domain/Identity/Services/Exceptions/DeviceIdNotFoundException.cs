using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class DeviceIdNotFoundException : IdentityDataValidationException
    {
        public DeviceId DeviceId { get; }

        public DeviceIdNotFoundException(DeviceId deviceId) 
            : base("Device with this id does not exist.")
        {
            DeviceId = deviceId;
        }
    }
}

using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public sealed class DeviceIdMismatchException : IdentityBusinessRuleValidationException
    {
        public DeviceId DeviceIdFromToken { get; }
        public DeviceId DeviceIdFromRefreshToken { get; }
        public DeviceId DeviceIdFromCurrentDevice { get; }

        public DeviceIdMismatchException(DeviceId deviceIdFromToken, DeviceId deviceIdFromRefreshToken, DeviceId deviceIdFromCurrentDevice)
            : base("Mismatch on device id from token and refresh token and/or requesting device.")
        {
            DeviceIdFromToken = deviceIdFromToken;
            DeviceIdFromRefreshToken = deviceIdFromRefreshToken;
            DeviceIdFromCurrentDevice = deviceIdFromCurrentDevice;
        }
    }
}

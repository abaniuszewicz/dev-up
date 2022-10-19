using DevUp.Infrastructure.Exceptions;
using DevUp.Infrastructure.Postgres.Identity.Dtos;

namespace DevUp.Infrastructure.Postgres.Identity.Repositories.Exceptions
{
    internal sealed class DeviceNotPersistedException : InfrastructureException
    {
        public DeviceDto Device { get; }

        public DeviceNotPersistedException(DeviceDto device)
            : base("Failed to persist device.")
        {
            Device = device;
        }
    }
}

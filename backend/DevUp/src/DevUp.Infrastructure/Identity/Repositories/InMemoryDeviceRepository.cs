using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;

namespace DevUp.Infrastructure.Identity.Repositories
{
    public class InMemoryDeviceRepository : IDeviceRepository
    {
        private readonly ConcurrentDictionary<DeviceId, Device> _devices = new();

        public Task AddAsync(Device device, CancellationToken cancellationToken)
        {
            _devices.TryAdd(device.Id, device);
            return Task.CompletedTask;
        }

        public Task<Device> GetByIdAsync(DeviceId deviceId, CancellationToken cancellationToken)
        {
            var device = _devices.TryGetValue(deviceId, out var result) ? result : null;
            return Task.FromResult(device);
        }
    }
}

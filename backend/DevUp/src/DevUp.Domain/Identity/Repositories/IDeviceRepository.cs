using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;

namespace DevUp.Domain.Identity.Repositories
{
    public interface IDeviceRepository
    {
        public Task AddAsync(Device device, CancellationToken cancellationToken);
        public Task<Device> GetByIdAsync(DeviceId deviceId, CancellationToken cancellationToken); 
    }
}

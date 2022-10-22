using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services
{
    public interface IIdentityService
    {
        public Task<TokenPair> RegisterAsync(Username username, Password password, Device device, CancellationToken cancellationToken);
        public Task<TokenPair> LoginAsync(Username username, Password password, Device device, CancellationToken cancellationToken);
    }
}

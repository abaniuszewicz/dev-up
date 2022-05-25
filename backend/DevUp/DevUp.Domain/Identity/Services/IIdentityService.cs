using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services
{
    public interface IIdentityService
    {
        public Task<IdentityResult> RegisterAsync(Username username, Password password, Device device, CancellationToken cancellationToken);
        public Task<IdentityResult> LoginAsync(Username username, Password password, Device device, CancellationToken cancellationToken);
        public Task<IdentityResult> RefreshAsync(Token token, RefreshTokenId refreshTokenId, Device device, CancellationToken cancellationToken);
    }
}

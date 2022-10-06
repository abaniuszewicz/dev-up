using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services
{
    public interface ITokenService
    {
        public Task<TokenPair> CreateAsync(UserId userId, DeviceId deviceId, CancellationToken cancellationToken);
        public Task<TokenPair> RefreshAsync(TokenPair tokenPair, Device device, CancellationToken cancellationToken);
        public Task RevokeAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
    }
}

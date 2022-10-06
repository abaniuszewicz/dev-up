using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;

namespace DevUp.Domain.Identity.Repositories
{
    public interface IRefreshTokenRepository
    {
        public Task<RefreshTokenInfo> AddAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken);
        public Task<RefreshTokenInfo> GetByIdAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        public Task<RefreshTokenInfo> UpdateAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken);
        public Task<RefreshTokenInfo> MarkAsUsedAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken);
        public Task InvalidateChainAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken);
    }
}

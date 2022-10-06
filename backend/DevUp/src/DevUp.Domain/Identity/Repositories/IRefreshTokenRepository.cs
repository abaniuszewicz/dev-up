using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;

namespace DevUp.Domain.Identity.Repositories
{
    public interface IRefreshTokenRepository
    {
        public Task AddAsync(RefreshTokenInfo refreshTokenInfo, CancellationToken cancellationToken);
        public Task<RefreshTokenInfo> GetByIdAsync(RefreshTokenInfoId refreshTokenInfoId, CancellationToken cancellationToken);
        public Task UpdateAsync(RefreshTokenInfo refreshTokenInfo, CancellationToken cancellationToken);
        public Task MarkAsUsedAsync(RefreshTokenInfo refreshTokenInfo, CancellationToken cancellationToken);
        public Task InvalidateChainAsync(RefreshTokenInfo refreshTokenInfo, CancellationToken cancellationToken);
    }
}

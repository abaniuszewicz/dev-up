using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;

namespace DevUp.Domain.Identity.Repositories
{
    public interface IRefreshTokenRepository
    {
        public Task AddAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken);
        public Task<RefreshTokenInfo> GetByIdAsync(RefreshTokenInfoId refreshToken, CancellationToken cancellationToken);
        public Task UpdateAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken);
        public Task MarkAsUsedAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken);
        public Task InvalidateChainAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken);
    }
}

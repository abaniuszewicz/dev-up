using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;

namespace DevUp.Domain.Identity.Repositories
{
    public interface IRefreshTokenRepository
    {
        public Task<RefreshToken> GetByIdAsync(RefreshTokenId id, CancellationToken cancellationToken);
        public Task<bool> ExistsAsync(RefreshToken token, CancellationToken cancellationToken);
    }
}

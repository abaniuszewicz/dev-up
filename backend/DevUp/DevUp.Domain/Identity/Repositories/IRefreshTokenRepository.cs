using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Repositories
{
    public interface IRefreshTokenRepository
    {
        public Task<UserId> GetUserIdAsync(RefreshToken token, CancellationToken cancellationToken);
        public Task<bool> ExistsAsync(RefreshToken token, CancellationToken cancellationToken);
    }
}

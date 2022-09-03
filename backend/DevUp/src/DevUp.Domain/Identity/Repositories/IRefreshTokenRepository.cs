using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.Repositories
{
    public interface IRefreshTokenRepository : IRepository<RefreshTokenInfo, RefreshToken>
    {
        public Task<RefreshTokenInfo> MarkAsUsedAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken);
    }
}

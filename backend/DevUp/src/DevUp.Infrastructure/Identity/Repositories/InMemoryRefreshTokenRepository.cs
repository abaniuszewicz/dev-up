using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;

namespace DevUp.Infrastructure.Identity.Repositories
{
    public class InMemoryRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ConcurrentDictionary<RefreshTokenInfoId, RefreshTokenInfo> _refreshTokens = new();

        public Task AddAsync(RefreshTokenInfo refreshTokenInfo, CancellationToken cancellationToken)
        {
            _refreshTokens.TryAdd(refreshTokenInfo.Id, refreshTokenInfo);
            return Task.CompletedTask;
        }

        public Task<RefreshTokenInfo> GetByIdAsync(RefreshTokenInfoId refreshTokenInfoId, CancellationToken cancellationToken)
        {
            var refreshTokenInfo = _refreshTokens.TryGetValue(refreshTokenInfoId, out var result) ? result: null;
            return Task.FromResult(refreshTokenInfo);
        }

        public Task InvalidateChainAsync(RefreshTokenInfo refreshTokenInfo, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task MarkAsUsedAsync(RefreshTokenInfo refreshTokenInfo, CancellationToken cancellationToken)
        {
            refreshTokenInfo.Used = true;
            return Task.CompletedTask;
        }

        public async Task UpdateAsync(RefreshTokenInfo refreshTokenInfo, CancellationToken cancellationToken)
        {
            _refreshTokens.TryRemove(refreshTokenInfo.Id, out _);
            await AddAsync(refreshTokenInfo, cancellationToken);
        }
    }
}

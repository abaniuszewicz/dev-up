using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.Services.Exceptions;
using DevUp.Infrastructure.Identity.Repositories.Dtos;

namespace DevUp.Infrastructure.Identity.Repositories
{
    public class InMemoryRefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ConcurrentDictionary<RefreshTokenInfoId, RefreshTokenDto> _refreshTokens = new();

        public Task AddAsync(RefreshTokenInfo refreshTokenInfo, CancellationToken cancellationToken)
        {
            var dto = new RefreshTokenDto() { RefreshTokenInfo = refreshTokenInfo };
            _refreshTokens.TryAdd(refreshTokenInfo.Id, dto);
            return Task.CompletedTask;
        }

        public Task<RefreshTokenInfo> GetByIdAsync(RefreshTokenInfoId refreshTokenInfoId, CancellationToken cancellationToken)
        {
            var dto = _refreshTokens.TryGetValue(refreshTokenInfoId, out var result) ? result: null;
            return Task.FromResult(dto?.RefreshTokenInfo);
        }

        public async Task ChainAsync(RefreshTokenInfoId oldRefreshTokenInfoId, RefreshTokenInfoId newRefreshTokenInfoId, CancellationToken cancellationToken)
        {
            if (!_refreshTokens.TryGetValue(oldRefreshTokenInfoId, out var oldDto))
                throw new RefreshTokenInfoIdNotFoundException(oldRefreshTokenInfoId);
            if (!_refreshTokens.TryGetValue(newRefreshTokenInfoId, out var newDto))
                throw new RefreshTokenInfoIdNotFoundException(newRefreshTokenInfoId);

            oldDto.Next = newRefreshTokenInfoId;
            newDto.Previous = oldRefreshTokenInfoId;
        }

        public Task InvalidateChainAsync(RefreshTokenInfo refreshTokenInfo, CancellationToken cancellationToken)
        {
            refreshTokenInfo.Invalidated = true;
            _refreshTokens.TryGetValue(refreshTokenInfo.Id, out var dto);
            while (dto?.Next is not null)
            {
                dto.RefreshTokenInfo.Invalidated = true;
                _refreshTokens.TryGetValue(dto.Next, out dto);
            }

            return Task.CompletedTask;
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

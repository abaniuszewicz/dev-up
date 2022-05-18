using DevUp.Infrastructure.Postgres.JwtIdentity.Dtos;
using Microsoft.AspNetCore.Identity;

namespace DevUp.Infrastructure.Postgres.JwtIdentity.Stores
{
    internal interface IRefreshTokenStore
    {
        public Task<IdentityResult> CreateAsync(RefreshTokenDto token, CancellationToken cancellationToken);
        public Task<RefreshTokenDto> GetAsync(string token, CancellationToken cancellationToken);
        public Task<IdentityResult> MarkAsUsed(RefreshTokenDto token, CancellationToken cancellationToken);
    }
}

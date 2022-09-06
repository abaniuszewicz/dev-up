using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services
{
    public interface ITokenService
    {
        public Task<(Token, RefreshToken)> CreateAsync(User user, Device device, CancellationToken cancellationToken);
        public Task<TokenInfo> DescribeAsync(Token token, CancellationToken cancellationToken);
        public Task<RefreshTokenInfo> DescribeAsync(RefreshToken refreshToken, CancellationToken cancellationToken);
        public Task ValidateAsync(TokenInfo token, RefreshTokenInfo refreshToken, Device device, CancellationToken cancellationToken);
    }
}

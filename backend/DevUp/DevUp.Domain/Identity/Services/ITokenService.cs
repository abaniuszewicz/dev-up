using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services
{
    public interface ITokenService
    {
        public Task ValidateAsync(Token token, RefreshToken refreshToken, Device device, CancellationToken cancellationToken);
    }
}

using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services
{
    public interface IIdentityService
    {
        public Task<IdentityResult> RegisterAsync(Username username, Password password, Device device);
        public Task<IdentityResult> LoginAsync(Username username, Password password, Device device);
        public Task<IdentityResult> RefreshAsync(Username token, Password refreshToken, Device device);
    }
}

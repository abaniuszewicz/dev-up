using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity
{
    public interface IIdentityService
    {
        /// <exception cref="RegistrationFailedException"/>
        public Task<IRegistrationResult> RegisterAsync(string username, string password, Device device);

        /// <exception cref="LoginFailedException"/>
        public Task<ILoginResult> LoginAsync(string username, string password, Device device);

        /// <exception cref="RefreshFailedException"/>
        public Task<IRefreshResult> RefreshAsync(string token, string refreshToken, Device device);
    }
}

using System;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;

namespace DevUp.Domain.Identity.Services
{
    internal class IdentityService : IIdentityService
    {
        public Task<ILoginResult> LoginAsync(string username, string password, Device device)
        {
            throw new NotImplementedException();
        }

        public Task<IRefreshResult> RefreshAsync(string token, string refreshToken, Device device)
        {
            throw new NotImplementedException();
        }

        public Task<IRegistrationResult> RegisterAsync(string username, string password, Device device)
        {
            throw new NotImplementedException();
        }
    }
}

using System.Threading.Tasks;
using DevUp.Domain.Identity;
using DevUp.Domain.Identity.Results;

namespace DevUp.Infrastructure.JwtIdentity
{
    internal class JwtIdentityService : IIdentityService
    {
        public Task<LoginResult> LoginAsync(string username, string password)
        {
            throw new System.NotImplementedException();
        }

        public Task<RegistrationResult> RegisterAsync(string username, string password)
        {
            throw new System.NotImplementedException();
        }
    }
}

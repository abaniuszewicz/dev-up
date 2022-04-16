using System.Threading.Tasks;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.Results;

namespace DevUp.Domain.Identity
{
    public interface IIdentityService
    {
        /// <summary>
        /// Registers an user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Authentication token</returns>
        /// <exception cref="RegistrationFailedException"/>
        public Task<RegistrationResult> RegisterAsync(string username, string password);

        /// <summary>
        /// Logs in an user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Authentication token</returns>
        /// <exception cref="LoginFailedException"/>
        public Task<LoginResult> LoginAsync(string username, string password);
    }
}

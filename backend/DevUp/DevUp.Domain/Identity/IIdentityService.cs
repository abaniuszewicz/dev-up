using System.Threading.Tasks;

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
        public Task<string> RegisterAsync(string username, string password);

        /// <summary>
        /// Logs in an user
        /// </summary>
        /// <param name="username">Username</param>
        /// <param name="password">Password</param>
        /// <returns>Authentication token</returns>
        /// <exception cref="LoginFailedException"/>
        public Task<string> LoginAsync(string username, string password);
    }
}

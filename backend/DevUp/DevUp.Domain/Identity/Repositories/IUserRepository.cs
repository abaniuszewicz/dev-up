using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Repositories
{
    public interface IUserRepository
    {
        public Task<User> CreateAsync(Username username, Password password);
        public Task<User> GetByUsernameAsync(Username username, CancellationToken cancellationToken);
        public Task<bool> CheckPasswordAsync(User user, Password password);
    }
}

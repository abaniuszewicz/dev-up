using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Infrastructure.Identity.Repositories
{
    public class InMemoryUserRepository : IUserRepository
    {
        private readonly ConcurrentDictionary<UserId, User> _userIdsToUsers = new();
        private readonly ConcurrentDictionary<Username, User> _usernamesToUsers = new();
        private readonly ConcurrentDictionary<UserId, PasswordHash> _userIdsToPasswordHashes = new();

        public Task<User> CreateAsync(Username username, PasswordHash passwordHash, CancellationToken cancellationToken)
        {
            var user = new User(new(), username);
            _userIdsToUsers.TryAdd(user.Id, user);
            _usernamesToUsers.TryAdd(username, user);
            _userIdsToPasswordHashes.TryAdd(user.Id, passwordHash);
            return Task.FromResult(user);
        }

        public Task<User> GetByIdAsync(UserId id, CancellationToken cancellationToken)
        {
            var user = _userIdsToUsers.TryGetValue(id, out var result) ? result : null;
            return Task.FromResult(user);
        }

        public Task<User> GetByUsernameAsync(Username username, CancellationToken cancellationToken)
        {
            var user = _usernamesToUsers.TryGetValue(username, out var result) ? result : null;
            return Task.FromResult(user);
        }

        public Task<PasswordHash> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            var passwordHash = _userIdsToPasswordHashes.TryGetValue(user.Id, out var result) ? result : null;
            return Task.FromResult(passwordHash);
        }
    }
}

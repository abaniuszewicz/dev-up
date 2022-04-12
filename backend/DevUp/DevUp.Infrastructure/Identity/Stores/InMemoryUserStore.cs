using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace DevUp.Infrastructure.Identity.Stores
{
    internal class InMemoryUserStore : IUserStore<User>, IUserPasswordStore<User>
    { 
        private readonly Dictionary<string, User> _store = new();

        #region IUserStore

        public Task<IdentityResult> CreateAsync(User user, CancellationToken cancellationToken)
        {
            var id = user.Id.ToString();

            if (_store.ContainsKey(id))
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "DUPLICATED_USER", Description = $"Cannot add user. User with id {id} already exist." }));

            _store.Add(id, user);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<IdentityResult> DeleteAsync(User user, CancellationToken cancellationToken)
        {
            var id = user.Id.ToString();

            if (!_store.ContainsKey(id))
                return Task.FromResult(IdentityResult.Failed(new IdentityError() { Code = "USER_DOESNT_EXIST", Description = $"Cannot delete user. User with id {id} doesn't exist." }));

            _store.Remove(id);
            return Task.FromResult(IdentityResult.Success);
        }

        public Task<User> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            var user = _store.TryGetValue(userId, out var result) ? result : null;
            return Task.FromResult(user);
        }

        public Task<User> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var user = _store.Values.FirstOrDefault(u => u.Username == normalizedUserName);
            return Task.FromResult(user);
        }

        public Task<string> GetNormalizedUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Username);
        }

        public Task<string> GetUserIdAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Username);
        }

        public Task SetNormalizedUserNameAsync(User user, string normalizedName, CancellationToken cancellationToken)
        {
            user.Username = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(User user, string userName, CancellationToken cancellationToken)
        {
            user.Username = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(IdentityResult.Success);
        }

        #endregion

        #region IPasswordStore

        public Task SetPasswordHashAsync(User user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task<string> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<bool> HasPasswordAsync(User user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash != null);
        }

        #endregion

        public void Dispose()
        {
        }
    }
}

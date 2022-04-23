using System.Data;
using Dapper;
using DevUp.Infrastructure.Postgres.JwtIdentity.Dtos;
using Microsoft.AspNetCore.Identity;

namespace DevUp.Infrastructure.Postgres.JwtIdentity.Stores
{
    internal class UserSTore : IUserStore<UserDto>, IUserPasswordStore<UserDto>
    {
        private readonly IDbConnection _connection;

        public UserSTore(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IdentityResult> CreateAsync(UserDto user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var sql = @"INSERT INTO users (id, username, normalized_username, password_hash)
                        VALUES (@Id, @UserName, @NormalizedUserName, @PasswordHash)";

            var affectedRows = await _connection.ExecuteAsync(sql, new { user.Id, user.UserName, user.NormalizedUserName, user.PasswordHash });
            return affectedRows > 0
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError() { Description = $"Could not create user {user.UserName}" });
        }

        public async Task<IdentityResult> DeleteAsync(UserDto user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var sql = @"DELETE FROM users
                        WHERE id=@Id";

            var affectedRows = await _connection.ExecuteAsync(sql, new { user.Id });
            return affectedRows > 0
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError() { Description = $"Could not delete user {user.UserName}" });
        }

        public async Task<UserDto> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentNullException(nameof(userId));

            var sql = @"SELECT *
                        FROM users
                        WHERE id=@Id";
            return await _connection.QuerySingleOrDefaultAsync<UserDto>(sql, new { Id = userId });
        }

        public async Task<UserDto> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(normalizedUserName))
                throw new ArgumentNullException(nameof(normalizedUserName));

            var sql = @"SELECT *
                        FROM users
                        WHERE normalized_username = @NormalizedUserName";

            return await _connection.QuerySingleOrDefaultAsync<UserDto>(sql, new { NormalizedUserName = normalizedUserName });
        }

        public Task<string> GetNormalizedUserNameAsync(UserDto user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.NormalizedUserName);
        }

        public Task<string> GetPasswordHashAsync(UserDto user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(UserDto user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(UserDto user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(UserDto user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            return Task.FromResult(user.PasswordHash is not null);
        }

        public Task SetNormalizedUserNameAsync(UserDto user, string normalizedName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            user.NormalizedUserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(UserDto user, string passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(UserDto user, string userName, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            user.UserName = userName;
            return Task.CompletedTask;
        }

        public async Task<IdentityResult> UpdateAsync(UserDto user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var sql = @"UPDATE users
                        SET username=@UserName, normalized_username=@NormalizedUserName, password_hash=@PasswordHash
                        WHERE id=@Id";

            var affectedRows = await _connection.ExecuteAsync(sql, new { user.Id, user.UserName, user.NormalizedUserName, user.PasswordHash }).ConfigureAwait(false);
            return affectedRows > 0
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError() { Description = $"Could not update user {user.UserName}" });
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }
    }
}

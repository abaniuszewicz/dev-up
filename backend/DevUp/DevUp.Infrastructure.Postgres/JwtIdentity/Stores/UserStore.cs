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
            var sql = @"INSERT INTO users (id, username, passwordhash)
                        VALUES (@Id, @UserName, @PasswordHash)";

            await _connection.ExecuteAsync(sql, new { Id = user.Id, UserName = user.UserName, PasswordHash = user.PasswordHash });
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(UserDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            _connection.Close();
            _connection.Dispose();
        }

        public Task<UserDto> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<UserDto> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken)
        {
            var sql = @"SELECT *
                        FROM users
                        WHERE UserName = @UserName";

            return await _connection.QuerySingleOrDefaultAsync<UserDto>(sql,
                new { UserName = normalizedUserName });
        }

        public Task<string> GetNormalizedUserNameAsync(UserDto user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<string> GetPasswordHashAsync(UserDto user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash);
        }

        public Task<string> GetUserIdAsync(UserDto user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.Id.ToString());
        }

        public Task<string> GetUserNameAsync(UserDto user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.UserName);
        }

        public Task<bool> HasPasswordAsync(UserDto user, CancellationToken cancellationToken)
        {
            return Task.FromResult(user.PasswordHash is not null);
        }

        public Task SetNormalizedUserNameAsync(UserDto user, string normalizedName, CancellationToken cancellationToken)
        {
            user.UserName = normalizedName;
            return Task.CompletedTask;
        }

        public Task SetPasswordHashAsync(UserDto user, string passwordHash, CancellationToken cancellationToken)
        {
            user.PasswordHash = passwordHash;
            return Task.CompletedTask;
        }

        public Task SetUserNameAsync(UserDto user, string userName, CancellationToken cancellationToken)
        {
            user.UserName = userName;
            return Task.CompletedTask;
        }

        public Task<IdentityResult> UpdateAsync(UserDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

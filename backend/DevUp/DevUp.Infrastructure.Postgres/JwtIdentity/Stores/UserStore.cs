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
            var sql = @"INSERT INTO users (Id, UserName, PasswordHash)
                        VALUES (@Id, @Name, @Hash)";

            await _connection.ExecuteAsync(sql, new { Id = user.Id, Name = user.UserName, Hash = user.PasswordHash });
            return IdentityResult.Success;
        }

        public Task<IdentityResult> DeleteAsync(UserDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
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
            return await _connection.QuerySingleAsync<UserDto>(sql,
                new { UserName = normalizedUserName });
        }

        public Task<string> GetNormalizedUserNameAsync(UserDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetPasswordHashAsync(UserDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(UserDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(UserDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<bool> HasPasswordAsync(UserDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(UserDto user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetPasswordHashAsync(UserDto user, string passwordHash, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(UserDto user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IdentityResult> UpdateAsync(UserDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

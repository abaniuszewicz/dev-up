using System.Data;
using Dapper;
using DevUp.Infrastructure.Postgres.JwtIdentity.Dtos;
using Microsoft.AspNetCore.Identity;

namespace DevUp.Infrastructure.Postgres.JwtIdentity.Stores
{
    internal class RefreshTokenStore : IRefreshTokenStore
    {
        private readonly IDbConnection _connection;

        public RefreshTokenStore(IDbConnection connection)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        public async Task<IdentityResult> CreateAsync(RefreshTokenDto token, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            _connection.Open();
            using var transaction = _connection.BeginTransaction();
            try
            {
                var devicesSql = @"INSERT INTO devices (id, name) VALUES (@Id, @Name) 
                                   ON CONFLICT (id) DO UPDATE SET name = @Name";
                var devicesAffectedRows = await _connection.ExecuteAsync(devicesSql, new { Id = token.Device.Id, Name = token.Device.Name });
                if (devicesAffectedRows <= 0)
                    throw new InvalidOperationException("Could not create device");

                var refreshTokensSql = @"INSERT INTO refresh_tokens (token, jti, user_id, creation_date, expiry_date, device_id, used, invalidated)
                        VALUES (@Token, @Jti, @UserId, @CreationDate, @ExpiryDate, @DeviceId, @Used, @Invalidated)";

                var refreshTokenAffectedRows = await _connection.ExecuteAsync(refreshTokensSql, 
                    new { Token = token.Token, Jti = token.Jti, UserId = token.UserId, CreationDate = token.CreationDate, ExpiryDate = token.ExpiryDate, DeviceId = token.Device.Id, Used = token.Used, Invalidated = token.Invalidated });
                if (refreshTokenAffectedRows <= 0)
                    throw new InvalidOperationException("Could not create refresh token");

                transaction.Commit();
                return IdentityResult.Success;
            }
            catch (Exception exception)
            {
                transaction.Rollback();
                return IdentityResult.Failed(new IdentityError() { Description = exception.Message });
            }
            finally
            {
                _connection.Close();
            }
        }

        public async Task<RefreshTokenDto> GetAsync(string token, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (string.IsNullOrWhiteSpace(token))
                throw new ArgumentNullException(nameof(token));

            var sql = @"SELECT rt.token Token, rt.jti Jti, rt.user_id UserId, rt.creation_date CreationDate, rt.expiry_date ExpiryDate, rt.device_id DeviceId, rt.used Used, rt.invalidated Invalidated, d.id Id, d.name Name
                        FROM refresh_tokens rt
                        INNER JOIN devices d ON rt.device_id = d.id
                        WHERE rt.token = @Token";

            return (await _connection.QueryAsync<RefreshTokenDto, DeviceDto, RefreshTokenDto>(sql, (refreshTokenDto, device) =>
            {
                refreshTokenDto.Device = device;
                return refreshTokenDto;
            }, new { Token = token }, splitOn: "Id")).Single();
        }

        public async Task<IdentityResult> MarkAsUsed(RefreshTokenDto token, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            var sql = @"UPDATE refresh_tokens
                        SET used = TRUE
                        WHERE token = @Token";

            var affectedRows = await _connection.ExecuteAsync(sql, new { Token = token.Token });
            return affectedRows >= 0
                ? IdentityResult.Success
                : IdentityResult.Failed(new IdentityError() { Description = $"Failed to mark refresh token as used." });
        }
    }
}

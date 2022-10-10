using System.Data;
using AutoMapper;
using Dapper;
using DevUp.Domain.Common.Extensions;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Infrastructure.Postgres.Identity.Dtos;
using DevUp.Infrastructure.Postgres.Identity.Repositories.Exceptions;
using DevUp.Infrastructure.Postgres.Setup;

namespace DevUp.Infrastructure.Postgres.Identity.Repositories
{
    internal class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;

        public RefreshTokenRepository(IDbConnectionFactory connectionFactory, IMapper mapper)
        {
            _connection = connectionFactory.Create(DbConnectionName.Identity);
            _mapper = mapper;
        }

        public async Task AddAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (refreshToken is null)
                throw new ArgumentNullException(nameof(refreshToken));

            var dto = _mapper.Map<RefreshTokenDto>(refreshToken);
            var sql = @$"INSERT INTO refresh_tokens (
                            token, 
                            jti, 
                            user_id, 
                            creation_date, 
                            expiry_date, 
                            device_id, 
                            used, 
                            invalidated
                        )
                        VALUES (
                            @{nameof(RefreshTokenDto.Token)}, 
                            @{nameof(RefreshTokenDto.Jti)}, 
                            @{nameof(RefreshTokenDto.UserId)}, 
                            @{nameof(RefreshTokenDto.CreationDate)}, 
                            @{nameof(RefreshTokenDto.ExpiryDate)}, 
                            @{nameof(RefreshTokenDto.DeviceId)}, 
                            @{nameof(RefreshTokenDto.Used)}, 
                            @{nameof(RefreshTokenDto.Invalidated)}
                        )";

            var affectedRows = await _connection.ExecuteAsync(sql, dto);
            if (affectedRows == 0)
                throw new TokenNotPersistedException(dto);
        }

        public async Task<RefreshTokenInfo?> GetByIdAsync(RefreshTokenInfoId refreshToken, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (refreshToken is null)
                throw new ArgumentNullException(nameof(refreshToken));

            var dto = new RefreshTokenDto() { Token = refreshToken.RefreshToken.Value };
            var sql = @$"SELECT 
                            token {nameof(RefreshTokenDto.Token)}, 
                            jti {nameof(RefreshTokenDto.Jti)}, 
                            user_id {nameof(RefreshTokenDto.UserId)}, 
                            creation_date {nameof(RefreshTokenDto.CreationDate)}, 
                            expiry_date {nameof(RefreshTokenDto.ExpiryDate)}, 
                            device_id {nameof(RefreshTokenDto.DeviceId)}, 
                            used {nameof(RefreshTokenDto.Used)}, 
                            invalidated {nameof(RefreshTokenDto.Invalidated)}
                        FROM refresh_tokens
                        WHERE token = @{nameof(RefreshTokenDto.Token)}";

            dto = await _connection.QuerySingleOrDefaultAsync<RefreshTokenDto>(sql, dto);
            return _mapper.MapOrNull<RefreshTokenInfo>(dto);
        }

        public async Task UpdateAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (refreshToken is null)
                throw new ArgumentNullException(nameof(refreshToken));

            var dto = _mapper.Map<RefreshTokenDto>(refreshToken);
            var sql = @$"UPDATE refresh_tokens
                        SET 
                            token = @{nameof(RefreshTokenDto.Token)}, 
                            jti = @{nameof(RefreshTokenDto.Jti)}, 
                            user_id = @{nameof(RefreshTokenDto.UserId)}, 
                            creation_date = @{nameof(RefreshTokenDto.CreationDate)}, 
                            expiry_date = @{nameof(RefreshTokenDto.ExpiryDate)}, 
                            device_id = @{nameof(RefreshTokenDto.DeviceId)}, 
                            used = @{nameof(RefreshTokenDto.Used)}, 
                            invalidated = @{nameof(RefreshTokenDto.Invalidated)}
                        WHERE token = @{nameof(RefreshTokenDto.Token)}";

            var affectedRows = await _connection.ExecuteAsync(sql, dto);
            if (affectedRows == 0)
                throw new TokenNotPersistedException(dto);
        }

        public async Task MarkAsUsedAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (refreshToken is null)
                throw new ArgumentNullException(nameof(refreshToken));

            var dto = _mapper.Map<RefreshTokenDto>(refreshToken);
            var sql = @$"UPDATE refresh_tokens
                        SET used = @{nameof(RefreshTokenDto.Used)}
                        WHERE token = @{nameof(RefreshTokenDto.Token)}";

            var affectedRows = await _connection.ExecuteAsync(sql, dto);
            if (affectedRows == 0)
                throw new TokenNotPersistedException(dto);

            refreshToken.Used = true;
        }

        public async Task InvalidateChainAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (refreshToken is null)
                throw new ArgumentNullException(nameof(refreshToken));

            var dto = _mapper.Map<RefreshTokenDto>(refreshToken);
            var sql = @$"";

            var affectedRows = await _connection.ExecuteAsync(sql, dto);
            if (affectedRows == 0)
                throw new TokenNotPersistedException(dto);

            refreshToken.Invalidated = true;
        }
    }
}

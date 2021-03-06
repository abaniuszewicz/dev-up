using System.Data;
using AutoMapper;
using Dapper;
using DevUp.Domain.Common.Extensions;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Infrastructure.Postgres.Identity.Dtos;
using DevUp.Infrastructure.Postgres.Setup;

namespace DevUp.Infrastructure.Postgres.Identity.Repositories
{
    internal class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;

        public RefreshTokenRepository(IDbConnectionFactory connectionFactory, IMapper mapper)
        {
            _connection = connectionFactory?.Create(DbConnectionName.Identity) ?? throw new ArgumentNullException(nameof(connectionFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<RefreshTokenInfo?> AddAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken)
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
            return affectedRows == 0 ? null : refreshToken;
        }

        public Task DeleteAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<RefreshTokenInfo>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<RefreshTokenInfo?> GetByIdAsync(RefreshToken refreshToken, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (refreshToken is null)
                throw new ArgumentNullException(nameof(refreshToken));

            var dto = new RefreshTokenDto() { Token = refreshToken.Value };
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

        public async Task<RefreshTokenInfo?> UpdateAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken)
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
            return affectedRows == 0 ? null : refreshToken;
        }

        public async Task<RefreshTokenInfo?> MarkAsUsedAsync(RefreshTokenInfo refreshToken, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (refreshToken is null)
                throw new ArgumentNullException(nameof(refreshToken));

            refreshToken.Used = true;
            var dto = _mapper.Map<RefreshTokenDto>(refreshToken);
            var sql = @$"UPDATE refresh_tokens
                        SET used = @{nameof(RefreshTokenDto.Used)}
                        WHERE token = @{nameof(RefreshTokenDto.Token)}";

            var affectedRows = await _connection.ExecuteAsync(sql, dto);
            return affectedRows == 0 ? null : refreshToken;
        }
    }
}

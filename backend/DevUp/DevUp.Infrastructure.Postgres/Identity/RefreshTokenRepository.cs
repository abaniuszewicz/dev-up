using System.Data;
using AutoMapper;
using Dapper;
using DevUp.Common.Extensions;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Infrastructure.Postgres.Identity.Dtos;

namespace DevUp.Infrastructure.Postgres.Identity
{
    internal class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;

        public RefreshTokenRepository(IDbConnection connection, IMapper mapper)
        {
            _connection = connection ?? throw new ArgumentNullException(nameof(connection));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<RefreshToken?> AddAsync(RefreshToken token, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            var dto = _mapper.Map<RefreshTokenDto>(token);
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
            return affectedRows == 0 ? null : token;
        }

        public Task DeleteAsync(RefreshToken entity, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<RefreshToken>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public async Task<RefreshToken?> GetByIdAsync(RefreshTokenId id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            var dto = new RefreshTokenDto() { Token = id.Token };
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
            return _mapper.MapOrNull<RefreshToken>(dto);
        }

        public async Task<RefreshToken?> UpdateAsync(RefreshToken token, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (token is null)
                throw new ArgumentNullException(nameof(token));

            var dto = _mapper.Map<RefreshTokenDto>(token);
            var sql = @$"UPDATE refresh_tokens
                        SET 
                            token @{nameof(RefreshTokenDto.Token)}, 
                            jti @{nameof(RefreshTokenDto.Jti)}, 
                            user_id @{nameof(RefreshTokenDto.UserId)}, 
                            creation_date @{nameof(RefreshTokenDto.CreationDate)}, 
                            expiry_date @{nameof(RefreshTokenDto.ExpiryDate)}, 
                            device_id @{nameof(RefreshTokenDto.DeviceId)}, 
                            used @{nameof(RefreshTokenDto.Used)}, 
                            invalidated @{nameof(RefreshTokenDto.Invalidated)}
                        WHERE token = @{nameof(RefreshTokenDto.Token)}";

            var affectedRows = await _connection.ExecuteAsync(sql, dto);
            return affectedRows == 0 ? null : token;
        }
    }
}

using System.Data;
using AutoMapper;
using Dapper;
using DevUp.Domain.Common.Extensions;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Infrastructure.Postgres.Identity.Dtos;
using DevUp.Infrastructure.Postgres.Identity.Repositories.Exceptions;
using DevUp.Infrastructure.Postgres.Setup;

namespace DevUp.Infrastructure.Postgres.Identity.Repositories
{
    internal class UserRepository : IUserRepository
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;

        public UserRepository(IDbConnectionFactory connectionFactory, IMapper mapper)
        {
            _connection = connectionFactory?.Create(DbConnectionName.Identity) ?? throw new ArgumentNullException(nameof(connectionFactory));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<User> CreateAsync(Username username, PasswordHash passwordHash, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (username is null)
                throw new ArgumentNullException(nameof(username));
            if (passwordHash is null)
                throw new ArgumentNullException(nameof(passwordHash));

            var id = new UserId();
            var user = new User(id, username);
            var dto = _mapper.Map<UserDto>(user, opt => opt.AfterMap((src, dest) => dest.PasswordHash = passwordHash.Value));

            var sql = @$"INSERT INTO users (
                            id, 
                            username, 
                            password_hash
                        )
                        VALUES (
                            @{nameof(UserDto.Id)}, 
                            @{nameof(UserDto.Username)}, 
                            @{nameof(UserDto.PasswordHash)}
                        )";

            var affectedRows = await _connection.ExecuteAsync(sql, dto);
            if (affectedRows == 0)
                throw new UserNotPersistedException(dto);

            return user;
        }

        public async Task<User?> GetByIdAsync(UserId id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            var dto = new UserDto() { Id = id.Id };
            var sql = @$"SELECT 
                            id {nameof(UserDto.Id)}, 
                            username {nameof(UserDto.Username)}, 
                            password_hash {nameof(UserDto.PasswordHash)}
                        FROM users
                        WHERE id=@{nameof(UserDto.Id)}";

            dto = await _connection.QuerySingleOrDefaultAsync<UserDto>(sql, dto);
            return _mapper.MapOrNull<User>(dto);
        }

        public async Task<User?> GetByUsernameAsync(Username username, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (username is null)
                throw new ArgumentNullException(nameof(username));

            var dto = new UserDto() { Username = username.Value };
            var sql = @$"SELECT 
                            id {nameof(UserDto.Id)}, 
                            username {nameof(UserDto.Username)}, 
                            password_hash {nameof(UserDto.PasswordHash)}
                        FROM users
                        WHERE username=@{nameof(UserDto.Username)}";

            dto = await _connection.QuerySingleOrDefaultAsync<UserDto>(sql, dto);
            return _mapper.MapOrNull<User>(dto);
        }

        public async Task<PasswordHash?> GetPasswordHashAsync(User user, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (user is null)
                throw new ArgumentNullException(nameof(user));

            var dto = new UserDto() { Id = user.Id.Id };
            var sql = @$"SELECT 
                            id {nameof(UserDto.Id)}, 
                            username {nameof(UserDto.Username)}, 
                            password_hash {nameof(UserDto.PasswordHash)}
                        FROM users
                        WHERE id=@{nameof(UserDto.Id)}";

            dto = await _connection.QuerySingleOrDefaultAsync<UserDto>(sql, dto);
            return _mapper.MapOrNull<PasswordHash>(dto);
        }
    }
}

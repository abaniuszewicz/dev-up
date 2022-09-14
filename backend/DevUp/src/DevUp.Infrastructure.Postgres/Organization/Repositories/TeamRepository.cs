using System.Data;
using AutoMapper;
using Dapper;
using DevUp.Domain.Common.Extensions;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Repositories;
using DevUp.Domain.Organization.ValueObjects;
using DevUp.Infrastructure.Postgres.Organization.Dtos;
using DevUp.Infrastructure.Postgres.Setup;

namespace DevUp.Infrastructure.Postgres.Organization.Repositories
{
    internal class TeamRepository : ITeamRepository
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;

        public TeamRepository(IDbConnectionFactory connectionFactory, IMapper mapper)
        {
            _connection = connectionFactory.Create(DbConnectionName.Organization);
            _mapper = mapper;
        }

        public async Task<IReadOnlyList<Team>> GetAllAsync(CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sql = @$"SELECT 
                            id {nameof(TeamDto.Id)}, 
                            name {nameof(TeamDto.Name)}
                        FROM users";

            var dtos = await _connection.QueryAsync<TeamDto>(sql);
            return dtos.Select(_mapper.MapOrNull<Team>).ToList();
        }

        public async Task<Team> GetByIdAsync(TeamId id, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (id is null)
                throw new ArgumentNullException(nameof(id));

            var dto = new TeamDto() { Id = id.Id };
            var sql = @$"SELECT 
                            id {nameof(TeamDto.Id)}, 
                            name {nameof(TeamDto.Name)}
                        FROM users
                        WHERE id=@{nameof(TeamDto.Id)}";

            dto = await _connection.QuerySingleOrDefaultAsync<TeamDto>(sql, dto);
            return _mapper.MapOrNull<Team>(dto);
        }

        public async Task<Team?> GetByNameAsync(TeamName name, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (name is null)
                throw new ArgumentNullException(nameof(name));

            var dto = new TeamDto() { Name = name.Value };
            var sql = @$"SELECT 
                            id {nameof(Team.Id)}, 
                            name {nameof(TeamDto.Name)}
                        FROM users
                        WHERE name=@{nameof(TeamDto.Name)}";

            dto = await _connection.QuerySingleOrDefaultAsync<TeamDto>(sql, dto);
            return _mapper.MapOrNull<Team>(dto);
        }

        public async Task<Team?> CreateAsync(Team team, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (team is null)
                throw new ArgumentNullException(nameof(team));

            var dto = _mapper.Map<TeamDto>(team);

            var sql = @$"INSERT INTO teams (
                            id, 
                            name
                        )
                        VALUES (
                            @{nameof(TeamDto.Id)}, 
                            @{nameof(TeamDto.Name)}
                        )";

            var affectedRows = await _connection.ExecuteAsync(sql, dto);
            return affectedRows == 0 ? null : team;
        }
    }
}

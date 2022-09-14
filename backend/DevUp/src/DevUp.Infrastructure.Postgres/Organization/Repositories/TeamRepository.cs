using System.Data;
using AutoMapper;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Repositories;
using DevUp.Domain.Organization.ValueObjects;
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

        public Task<IReadOnlyList<Team>> GetAllAsync(CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Team> GetByIdAsync(TeamId id, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Team> GetByNameAsync(TeamName name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<Team> CreateAsync(Team name, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

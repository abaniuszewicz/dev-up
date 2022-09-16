using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Repositories;
using DevUp.Domain.Organization.Services.Exceptions;
using DevUp.Domain.Organization.ValueObjects;

namespace DevUp.Domain.Organization.Services
{
    internal class TeamService : ITeamService
    {
        private readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<Team> CreateAsync(TeamId id, TeamName name, CancellationToken cancellationToken)
        {
            var (byId, byName) = (_teamRepository.GetByIdAsync(id, cancellationToken), _teamRepository.GetByNameAsync(name, cancellationToken));
            await Task.WhenAll(byId, byName);
            if (byId.Result is not null)
                throw new TeamIdTakenException(id);
            if (byName.Result is not null)
                throw new TeamNameTakenException(name);

            var team = new Team(id, name);
            return await _teamRepository.CreateAsync(team, cancellationToken);
        }

        public async Task<Team> GetAsync(TeamId id, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(id, cancellationToken);
            if (team is null)
                throw new TeamIdNotFoundException(id);

            return team;
        }

        public async Task DeleteAsync(TeamId id, CancellationToken cancellationToken)
        {
            var team = await _teamRepository.GetByIdAsync(id, cancellationToken);
            if (team is null)
                throw new TeamIdNotFoundException(id);

            await _teamRepository.DeleteAsync(team, cancellationToken);
        }
    }
}

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.ValueObjects;

namespace DevUp.Domain.Organization.Repositories
{
    public interface ITeamRepository
    {
        public Task<IReadOnlyList<Team>> GetAllAsync(CancellationToken cancellationToken);
        public Task<Team> GetByIdAsync(TeamId id, CancellationToken cancellationToken);
        public Task<Team> GetByNameAsync(TeamName name, CancellationToken cancellationToken);
        public Task<Team> CreateAsync(Team team, CancellationToken cancellationToken);
        public Task DeleteAsync(Team team, CancellationToken cancellationToken);
    }
}

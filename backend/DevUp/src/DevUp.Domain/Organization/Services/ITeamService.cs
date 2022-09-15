using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.ValueObjects;

namespace DevUp.Domain.Organization.Services
{
    public interface ITeamService
    {
        public Task<Team> CreateAsync(TeamId id, TeamName name, CancellationToken cancellationToken);
        public Task<Team> GetAsync(TeamId id, CancellationToken cancellationToken);
    }
}

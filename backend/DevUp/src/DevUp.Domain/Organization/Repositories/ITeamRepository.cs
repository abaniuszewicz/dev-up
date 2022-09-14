using System.Collections.Generic;
using System.Threading.Tasks;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.ValueObjects;

namespace DevUp.Domain.Organization.Repositories
{
    public interface ITeamRepository
    {
        public Task<IReadOnlyList<Team>> GetAllAsync();
        public Task<Team> GetByIdAsync(TeamId id);
        public Task<Team> GetByNameAsync(TeamName name);
        public Task<Team> CreateAsync(TeamName name);
    }
}

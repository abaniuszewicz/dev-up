using System.Threading.Tasks;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.ValueObjects;

namespace DevUp.Domain.Organization.Services
{
    public interface ITeamService
    {
        public Task<Team> Create(TeamName name);
    }
}

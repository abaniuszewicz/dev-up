using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Organization.Entities;

namespace DevUp.Domain.Organization.Services
{
    public interface IMemberService
    {
        public Task<MemberInvitation> CreateAsync(MemberInvitationId memberInvitationId, TeamId teamId, CancellationToken cancellationToken);
    }
}

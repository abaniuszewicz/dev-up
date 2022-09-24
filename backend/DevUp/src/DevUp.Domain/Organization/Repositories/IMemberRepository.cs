using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Organization.Entities;

namespace DevUp.Domain.Organization.Repositories
{
    public interface IMemberRepository
    {
        public Task<Member> GetByIdAsync(MemberInvitation memberInvitation, CancellationToken cancellationToken);
        public Task<MemberInvitation> CreateAsync(MemberInvitation memberInvitation, CancellationToken cancellationToken);
    }
}

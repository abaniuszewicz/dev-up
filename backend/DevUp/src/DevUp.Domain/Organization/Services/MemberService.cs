using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Repositories;

namespace DevUp.Domain.Organization.Services
{
    internal class MemberService : IMemberService
    {
        private readonly IMemberRepository _memberRepository;

        public MemberService(IMemberRepository memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<MemberInvitation> CreateAsync(MemberInvitationId memberInvitationId, TeamId teamId, CancellationToken cancellationToken)
        {
            var memberInvitation = new MemberInvitation(memberInvitationId, teamId.Id);
            var receiver = await _memberRepository.GetByIdAsync(memberInvitation, cancellationToken);
            if (receiver is not null && receiver.TeamId == teamId)
                throw new System.Exception("Already assigned exception");
                //throw new MemberAlreadyAssignedException(receiver);

            //var team = _teamRepository.GetByIdAsync(invitation.TeamId);
            //if (team.OwnerId != invitation.Id.SenderId)
                //throw new TeamInsufficientPermission(invitation.Id.SenderId);

            return await _memberRepository.CreateAsync(memberInvitation, cancellationToken);
        }
    }
}

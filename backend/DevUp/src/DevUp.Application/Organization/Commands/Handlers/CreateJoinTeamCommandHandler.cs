using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Services;
using MediatR;

namespace DevUp.Application.Organization.Commands.Handlers
{
    internal class CreateJoinTeamCommandHandler : IRequestHandler<CreateJoinTeamCommand>
    {
        private readonly IMemberService _memberService;
        public CreateJoinTeamCommandHandler(IMemberService memberService)
        {
            _memberService = memberService;
        }

        public async Task<Unit> Handle(CreateJoinTeamCommand request, CancellationToken cancellationToken)
        {
            var memberInvitationId = new MemberInvitationId(request.SenderId, request.ReceiverId);

            await _memberService.CreateAsync(memberInvitationId, new TeamId(request.TeamId), cancellationToken);
            return default;
        }
    }
}

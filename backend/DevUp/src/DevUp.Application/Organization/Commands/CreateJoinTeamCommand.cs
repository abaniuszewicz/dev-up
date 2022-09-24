using MediatR;

namespace DevUp.Application.Organization.Commands
{
    public class CreateJoinTeamCommand : IRequest
    {
        public Guid SenderId { get; init; }
        public Guid ReceiverId { get; init; }
        public Guid TeamId { get; set; }
    }
}

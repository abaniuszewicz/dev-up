using MediatR;

namespace DevUp.Application.Organization.Commands
{
    public sealed class DeleteTeamCommand : IRequest
    {
        public Guid Id { get; init; }
    }
}

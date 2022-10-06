using MediatR;

namespace DevUp.Application.Organization.Commands
{
    public class CreateTeamCommand : IRequest
    {
        public Guid Id { get; init; } = Guid.NewGuid();
        public string Name { get; init; }
    }
}

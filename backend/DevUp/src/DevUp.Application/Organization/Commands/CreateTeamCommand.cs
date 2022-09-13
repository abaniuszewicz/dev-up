using MediatR;

namespace DevUp.Application.Organization.Commands
{
    public class CreateTeamCommand : IRequest
    {
        public Guid Id { get; } = Guid.NewGuid();
        public string Name { get; init; }
    }
}

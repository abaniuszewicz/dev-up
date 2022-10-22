using MediatR;

namespace DevUp.Application.Organization.Commands
{
    public sealed class UpdateTeamCommand : IRequest
    {
        public Guid Id { get; set; }
        public string Name { get; init; }
    }
}

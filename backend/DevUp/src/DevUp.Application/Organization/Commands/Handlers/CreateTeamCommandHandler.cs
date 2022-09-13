using MediatR;

namespace DevUp.Application.Organization.Commands.Handlers
{
    internal class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand>
    {
        public Task<Unit> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}

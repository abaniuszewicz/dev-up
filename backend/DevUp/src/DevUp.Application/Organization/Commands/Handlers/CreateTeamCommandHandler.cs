using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Services;
using DevUp.Domain.Organization.ValueObjects;
using MediatR;

namespace DevUp.Application.Organization.Commands.Handlers
{
    internal class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand>
    {
        private readonly ITeamService _teamService;

        public CreateTeamCommandHandler(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<Unit> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var id = new TeamId(request.Id);
            var name = new TeamName(request.Name);
            await _teamService.CreateAsync(id, name, cancellationToken);
            return default;
        }
    }
}

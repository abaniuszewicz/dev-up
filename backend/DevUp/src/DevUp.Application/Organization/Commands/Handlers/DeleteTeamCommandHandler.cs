using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Services;
using MediatR;

namespace DevUp.Application.Organization.Commands.Handlers
{
    internal class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand>
    {
        private readonly ITeamService _teamService;

        public DeleteTeamCommandHandler(ITeamService teamService)
        {
            _teamService = teamService;
        }

        public async Task<Unit> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
        {
            var id = new TeamId(request.Id);
            await _teamService.DeleteAsync(id, cancellationToken);
            return default;
        }
    }
}

using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Repositories;
using DevUp.Domain.Organization.Services.Exceptions;
using DevUp.Domain.Organization.ValueObjects;
using MediatR;

namespace DevUp.Application.Organization.Commands.Handlers
{
    internal class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand>
    {
        private readonly ITeamRepository _teamRepository;

        public UpdateTeamCommandHandler(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<Unit> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
        {
            var id = new TeamId(request.Id);
            var name = new TeamName(request.Name);

            var team = await _teamRepository.GetByIdAsync(id, cancellationToken);
            if (team is null)
                throw new TeamIdNotFoundException(id);

            var teamWithTheSameName = await _teamRepository.GetByNameAsync(name, cancellationToken);
            if (teamWithTheSameName == team)
                return default;
            if (teamWithTheSameName is not null)
                throw new TeamNameTakenException(name);

            team.ChangeName(name);
            await _teamRepository.UpdateAsync(team, cancellationToken);

            return default;
        }
    }
}

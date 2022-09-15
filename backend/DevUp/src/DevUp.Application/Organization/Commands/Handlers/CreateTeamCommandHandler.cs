using AutoMapper;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Services;
using DevUp.Domain.Organization.ValueObjects;
using MediatR;

namespace DevUp.Application.Organization.Commands.Handlers
{
    internal class CreateTeamCommandHandler : IRequestHandler<CreateTeamCommand>
    {
        private readonly ITeamService _teamService;
        private readonly IMapper _mapper;

        public CreateTeamCommandHandler(ITeamService teamService, IMapper mapper)
        {
            _teamService = teamService;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(CreateTeamCommand request, CancellationToken cancellationToken)
        {
            var id = new TeamId(request.Id);
            var name = new TeamName(request.Name);
            var team = await _teamService.CreateAsync(id, name, cancellationToken);
            return default;
        }
    }
}

using AutoMapper;
using DevUp.Application.Organization.Queries.Results;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Services;
using MediatR;

namespace DevUp.Application.Organization.Queries.Handlers
{
    internal sealed class GetTeamQueryHandler : IRequestHandler<GetTeamQuery, TeamQueryResult>
    {
        private readonly ITeamService _teamService;
        private readonly IMapper _mapper;

        public GetTeamQueryHandler(ITeamService teamService, IMapper mapper)
        {
            _teamService = teamService;
            _mapper = mapper;
        }

        public async Task<TeamQueryResult> Handle(GetTeamQuery request, CancellationToken cancellationToken)
        {
            var id = new TeamId(request.Id);
            var team = await _teamService.GetAsync(id, cancellationToken);
            var result = _mapper.Map<TeamQueryResult>(team);
            return result;
        }
    }
}

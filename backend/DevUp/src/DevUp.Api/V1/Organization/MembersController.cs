using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Threading;
using DevUp.Api.Contracts;
using AutoMapper;
using MediatR;
using DevUp.Api.Contracts.V1.Organization.Requests;
using DevUp.Application.Organization.Commands;
using System;

namespace DevUp.Api.V1.Organization
{
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        public MembersController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpPost(Route.Api.V1.TeamMembers.Create)]
        public async Task<IActionResult> CreateJoinRequest([FromRoute] Guid teamId,[FromBody] CreateJoinTeamRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateJoinTeamCommand>(request, opts => opts.AfterMap((_, c) => c.TeamId = teamId));
            await _mediator.Send(command, cancellationToken);
            // should we send some notifications to user here?
            return Ok("Invitation created");
        }
    }
}

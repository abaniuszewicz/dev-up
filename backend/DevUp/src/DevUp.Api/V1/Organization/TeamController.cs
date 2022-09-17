using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevUp.Api.Contracts;
using DevUp.Api.Contracts.V1.Organization.Requests;
using DevUp.Api.Contracts.V1.Organization.Responses;
using DevUp.Application.Organization.Commands;
using DevUp.Application.Organization.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevUp.Api.V1.Organization
{
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public TeamController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpGet(Route.Api.V1.Teams.GetAll)]
        public async Task<IActionResult> GetAllTeams(CancellationToken cancellationToken)
        {
            var query = new GetAllTeamsQuery();
            var result = await _mediator.Send(query, cancellationToken);
            var response = _mapper.Map<IEnumerable<TeamResponse>>(result);
            return Ok(response);
        }

        [HttpGet(Route.Api.V1.Teams.GetById)]
        public async Task<IActionResult> GetTeamById([FromRoute] Guid teamId, CancellationToken cancellationToken)
        {
            var query = new GetTeamQuery() { Id = teamId };
            var result = await _mediator.Send(query, cancellationToken);
            var response = _mapper.Map<TeamResponse>(result);
            return Ok(response);
        }

        [HttpPost(Route.Api.V1.Teams.Create)]
        public async Task<IActionResult> Create([FromBody] CreateTeamRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<CreateTeamCommand>(request);
            await _mediator.Send(command, cancellationToken);
            return CreatedAtAction(nameof(GetTeamById), new { TeamId = command.Id }, null);
        }

        [HttpPut(Route.Api.V1.Teams.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid teamId, [FromBody] UpdateTeamRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<UpdateTeamCommand>(request, opts => opts.AfterMap((_, c) => c.Id = teamId));
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpDelete(Route.Api.V1.Teams.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid teamId, CancellationToken cancellationToken)
        {
            var command = new DeleteTeamCommand() { Id = teamId };
            await _mediator.Send(command, cancellationToken);
            return NoContent();
        }
    }
}

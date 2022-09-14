﻿using System;
using System.Threading.Tasks;
using AutoMapper;
using DevUp.Api.Contracts;
using DevUp.Api.Contracts.V1.Organization.Requests;
using DevUp.Application.Organization.Commands;
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
        public async Task<IActionResult> GetAllTeams()
        {
            throw new NotImplementedException();
        }

        [HttpGet(Route.Api.V1.Teams.GetById)]
        public async Task<IActionResult> GetTeamById([FromRoute] Guid teamId)
        {
            throw new NotImplementedException();
        }

        [HttpPost(Route.Api.V1.Teams.Create)]
        public async Task<IActionResult> Create([FromBody] CreateTeamRequest request)
        {
            var command = _mapper.Map<CreateTeamCommand>(request);
            await _mediator.Send(command);
            return CreatedAtAction(nameof(GetTeamById), command.Id);
        }

        [HttpPatch(Route.Api.V1.Teams.Update)]
        public async Task<IActionResult> Update([FromRoute] Guid teamId)
        {
            throw new NotImplementedException();
        }

        [HttpDelete(Route.Api.V1.Teams.Delete)]
        public async Task<IActionResult> Delete([FromRoute] Guid teamId)
        {
            throw new NotImplementedException();
        }
    }
}

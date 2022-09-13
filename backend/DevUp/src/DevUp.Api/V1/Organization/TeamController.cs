using System;
using System.Threading.Tasks;
using AutoMapper;
using DevUp.Api.Contracts;
using DevUp.Api.Contracts.V1.Organization.Requests;
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
        public async Task<IActionResult> GetTeamById(Guid teamId)
        {
            throw new NotImplementedException();
        }

        [HttpPost(Route.Api.V1.Teams.Create)]
        public async Task<IActionResult> Create(CreateTeamRequest request)
        {
            throw new NotImplementedException();
        }

        [HttpPatch(Route.Api.V1.Teams.Update)]
        public async Task<IActionResult> Update(Guid teamId)
        {
            throw new NotImplementedException();
        }

        [HttpDelete(Route.Api.V1.Teams.Delete)]
        public async Task<IActionResult> Delete(Guid teamId)
        {
            throw new NotImplementedException();
        }
    }
}

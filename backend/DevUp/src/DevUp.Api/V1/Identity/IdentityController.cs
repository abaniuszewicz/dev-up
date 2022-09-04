using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevUp.Api.Contracts;
using DevUp.Api.Contracts.V1.Identity.Requests;
using DevUp.Application.Identity.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevUp.Api.V1.Identity
{
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public IdentityController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [HttpPost(Route.Api.V1.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<RegisterUserCommand>(request);
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpPost(Route.Api.V1.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<LoginUserCommand>(request);
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }

        [HttpPost(Route.Api.V1.Identity.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshUserRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<RefreshUserCommand>(request);
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }
    }
}

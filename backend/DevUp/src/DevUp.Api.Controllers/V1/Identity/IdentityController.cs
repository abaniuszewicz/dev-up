using AutoMapper;
using DevUp.Api.Contracts.V1.Identity.Requests;
using DevUp.Api.Contracts.V1.Identity.Responses;
using DevUp.Application.Identity;
using DevUp.Application.Identity.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevUp.Api.Controllers.V1.Identity
{
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ITokenStore _tokenStore;

        public IdentityController(IMapper mapper, IMediator mediator, ITokenStore tokenStore)
        {
            _mapper = mapper;
            _mediator = mediator;
            _tokenStore = tokenStore;
        }

        [HttpPost(Contracts.Route.Api.V1.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<RegisterUserCommand>(request);
            await _mediator.Send(command, cancellationToken);

            var tokenPair = _tokenStore.Get();
            var response = _mapper.Map<IdentityResponse>(tokenPair);
            return Ok(response);
        }

        [HttpPost(Contracts.Route.Api.V1.Identity.Login)]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<LoginUserCommand>(request);
            await _mediator.Send(command, cancellationToken);

            var tokenPair = _tokenStore.Get();
            var response = _mapper.Map<IdentityResponse>(tokenPair);
            return Ok(response);
        }

        [HttpPost(Contracts.Route.Api.V1.Identity.Refresh)]
        public async Task<IActionResult> Refresh([FromBody] RefreshUserRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<RefreshUserCommand>(request);
            await _mediator.Send(command, cancellationToken);

            var tokenPair = _tokenStore.Get();
            var response = _mapper.Map<IdentityResponse>(tokenPair);
            return Ok(response);
        }

        [HttpPost(Contracts.Route.Api.V1.Identity.Revoke)]
        public async Task<IActionResult> Revoke([FromBody] RevokeTokenRequest request, CancellationToken cancellationToken)
        {
            var command = _mapper.Map<RevokeTokenCommand>(request);
            await _mediator.Send(command, cancellationToken);
            return Ok();
        }
    }
}

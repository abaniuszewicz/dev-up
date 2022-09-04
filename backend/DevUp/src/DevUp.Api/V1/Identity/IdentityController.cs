using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevUp.Api.Contracts;
using DevUp.Api.Contracts.V1.Identity.Requests;
using DevUp.Api.Contracts.V1.Identity.Responses;
using DevUp.Application.Identity.Commands;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Seedwork.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DevUp.Api.V1.Identity
{
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public IdentityController(IIdentityService identityService, IMapper mapper, IMediator mediator)
        {
            _identityService = identityService;
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var token = _mapper.Map<Token>(request);
                var refreshToken = _mapper.Map<RefreshToken>(request);
                var device = _mapper.Map<Device>(request.Device);

                var result = await _identityService.RefreshAsync(token, refreshToken, device, cancellationToken);
                var response = _mapper.Map<IdentityResponse>(result);
                return Ok(response);
            }
            catch (DomainException exception)
            {
                var response = _mapper.Map<IdentityResponse>(exception);
                return BadRequest(response);
            }
            catch (OperationCanceledException exception)
            {
                return BadRequest();
            }
            catch (Exception exception)
            {
                return Problem();
            }
        }
    }
}

using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using DevUp.Api.V1.Controllers.Identity.Requests;
using DevUp.Api.V1.Controllers.Identity.Responses;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Seedwork.Exceptions;
using Microsoft.AspNetCore.Mvc;

namespace DevUp.Api.V1.Controllers.Identity
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IMapper _mapper;

        public IdentityController(IIdentityService identityService, IMapper mapper)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var username = new Username(request.Username);
                var password = new Password(request.Password);
                var device = new Device(new DeviceId(request.Device.Id), request.Device.Name);

                var result = await _identityService.RegisterAsync(username, password, device, cancellationToken);
                var response = IdentityResponse.Succeeded(result);
                return Ok(response);
            }
            catch (ValidationException exception)
            {
                return BadRequest(exception.Errors);
            }
            catch (OperationCanceledException exception)
            {
                return BadRequest();
            }
            catch (IdentityException exception)
            {
                var response = IdentityResponse.Failed(exception);
                return BadRequest(response);
            }
            catch (Exception exception)
            {
                return Problem();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var username = new Username(request.Username);
                var password = new Password(request.Password);
                var device = new Device(new DeviceId(request.Device.Id), request.Device.Name);

                var result = await _identityService.LoginAsync(username, password, device, cancellationToken);
                var response = IdentityResponse.Succeeded(result);
                return Ok(response);
            }
            catch (ValidationException exception)
            {
                return BadRequest(exception.Errors);
            }
            catch (OperationCanceledException exception)
            {
                return BadRequest();
            }
            catch (IdentityException exception)
            {
                var response = IdentityResponse.Failed(exception);
                return BadRequest(response);
            }
            catch (Exception exception)
            {
                return Problem();
            }
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshUserRequest request, CancellationToken cancellationToken)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var token = new Token(request.Token);
                var refreshTokenId = new RefreshTokenId(request.RefreshToken);
                var device = new Device(new DeviceId(request.Device.Id), request.Device.Name);

                var result = await _identityService.RefreshAsync(token, refreshTokenId, device, cancellationToken);
                var response = IdentityResponse.Succeeded(result);
                return Ok(response);
            }
            catch (ValidationException exception)
            {
                return BadRequest(exception.Errors);
            }
            catch (OperationCanceledException exception)
            {
                return BadRequest();
            }
            catch (IdentityException exception)
            {
                var response = IdentityResponse.Failed(exception);
                return BadRequest(response);
            }
            catch (Exception exception)
            {
                return Problem();
            }
        }
    }
}

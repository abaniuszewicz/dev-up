using System;
using System.Threading.Tasks;
using DevUp.Api.V1.Controllers.Identity.Requests;
using DevUp.Api.V1.Controllers.Identity.Responses;
using DevUp.Domain.Identity;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Infrastructure.Postgres.JwtIdentity.Results;
using Microsoft.AspNetCore.Mvc;

namespace DevUp.Api.V1.Controllers.Identity
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService identityService)
        {
            _identityService = identityService ?? throw new ArgumentNullException(nameof(identityService));
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _identityService.RegisterAsync(request.Username, request.Password) as JwtRegistrationResult;
                var response = new RegistrationSucceededResponse() { Token = result.Token };
                return Ok(response);
            }
            catch (RegistrationFailedException exception)
            {
                var response = new RegistrationFailedResponse() { Errors = exception.Errors };
                return BadRequest(response);
            }
            catch (Exception exception)
            {
                return Problem();
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginUserRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var result = await _identityService.LoginAsync(request.Username, request.Password) as JwtLoginResult;
                var response = new LoginSucceededResponse() { Token = result.Token };
                return Ok(response);
            }
            catch (LoginFailedException exception)
            {
                var response = new LoginFailedResponse() { Errors = exception.Errors };
                return BadRequest(response);
            }
            catch (Exception exception)
            {
                return Problem();
            }
        }
    }
}

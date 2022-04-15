using System;
using System.Threading.Tasks;
using DevUp.Api.V1.Controllers.Identity.Requests;
using DevUp.Api.V1.Controllers.Identity.Responses;
using DevUp.Domain.Identity;
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
                var token = await _identityService.RegisterAsync(request.Username, request.Password);
                var response = new RegistrationSucceededResponse() { Token = token };
                return Ok(response);
            }
            catch (RegistrationFailedException exception)
            {
                var response = new RegistrationFailedResponse() { Errors = exception.Errors };
                return BadRequest(response);
            }
            catch
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
                var token = await _identityService.LoginAsync(request.Username, request.Password);
                var response = new LoginSucceededResponse() { Token = token };
                return Ok(response);
            }
            catch (LoginFailedException exception)
            {
                var response = new LoginFailedResponse() { Errors = exception.Errors };
                return BadRequest(response);
            }
            catch
            {
                return Problem();
            }
        }
    }
}

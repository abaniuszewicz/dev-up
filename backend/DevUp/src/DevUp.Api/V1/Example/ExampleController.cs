using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevUp.Api.Contracts;
using DevUp.Api.Contracts.V1.Example.Requests;
using DevUp.Api.Contracts.V1.Example.Responses;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DevUp.Api.V1.Example
{
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ExampleController : ControllerBase
    {
        [HttpGet(Route.Api.V1.Example.Url)]
        public async Task<IEnumerable<ExampleResponse>> Get()
        {
            return await Task.FromResult(Enumerable.Repeat(new ExampleResponse(), 5));
        }

        [HttpPost(Route.Api.V1.Example.Url)]
        public IActionResult Post([FromBody] ExampleRequest request)
        {
            return Ok();
        }
    }
}

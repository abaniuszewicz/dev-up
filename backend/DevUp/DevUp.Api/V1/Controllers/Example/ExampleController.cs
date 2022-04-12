using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DevUp.Api.V1.Controllers.Example.Requests;
using DevUp.Api.V1.Controllers.Example.Responses;
using Microsoft.AspNetCore.Mvc;

namespace DevUp.Api.V1.Controllers.Example
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ExampleController : ControllerBase
    {
        [HttpGet]
        public async Task<IEnumerable<ExampleResponse>> Get()
        {
            return await Task.FromResult(Enumerable.Repeat(new ExampleResponse(), 5));
        }

        [HttpPost]
        public IActionResult Post([FromBody] ExampleRequest request)
        {
            return Ok();
        }
    }
}

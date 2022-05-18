using System.Collections.Generic;

namespace DevUp.Api.V1.Controllers.Identity.Responses
{
    public abstract class FailedResponse
    {
        public IEnumerable<string> Errors { get; init; }
    }
}

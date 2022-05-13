using System.Collections.Generic;

namespace DevUp.Api.V1.Controllers.Identity.Responses
{
    public class RegistrationFailedResponse
    {
        public IEnumerable<string> Errors { get; init; }
    }
}

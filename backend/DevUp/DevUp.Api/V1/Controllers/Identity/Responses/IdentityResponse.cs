using System.Collections.Generic;
using System.Linq;
using DevUp.Domain.Identity;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Api.V1.Controllers.Identity.Responses
{
    public class IdentityResponse
    {
        public bool Success { get; }
        public string[] Errors { get; }
        public string? Token { get;}
        public string? RefreshToken { get; }

        internal static IdentityResponse Succeeded(IdentityResult result)
        {
            return new(true, Enumerable.Empty<string>(), result.Token.Value, result.RefreshToken.Value);
        }

        internal static IdentityResponse Failed(IdentityException exception)
        {
            return new(false, exception.Errors, null, null);
        }

        private IdentityResponse(bool success, IEnumerable<string> errors, string? token, string? refreshToken)
        {
            Success = success;
            Errors = errors.ToArray();
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}

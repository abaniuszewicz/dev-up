using DevUp.Domain.Identity.Results;

namespace DevUp.Infrastructure.JwtIdentity.Results
{
    public class JwtLoginResult : LoginResult
    {
        public string Token { get; init; }
    }
}

using DevUp.Domain.Identity.Results;

namespace DevUp.Infrastructure.Postgres.JwtIdentity.Results
{
    public class JwtLoginResult : LoginResult
    {
        public string Token { get; init; }
    }
}

using DevUp.Domain.Identity.Results;

namespace DevUp.Infrastructure.JwtIdentity.Results
{
    public class JwtRegistrationResult : RegistrationResult
    {
        public string Token { get; init; }
    }
}

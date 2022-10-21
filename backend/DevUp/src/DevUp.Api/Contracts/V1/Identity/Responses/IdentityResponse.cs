namespace DevUp.Api.Contracts.V1.Identity.Responses
{
    public class IdentityResponse
    {
        public string Token { get; init; }
        public string RefreshToken { get; init; }
    }
}

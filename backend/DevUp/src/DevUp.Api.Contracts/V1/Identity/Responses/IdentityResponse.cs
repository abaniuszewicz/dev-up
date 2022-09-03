namespace DevUp.Api.Contracts.V1.Identity.Responses
{
    public class IdentityResponse
    {
        public bool Success { get; init; }
        public string[] Errors { get; init; }
        public string? Token { get; init; }
        public string? RefreshToken { get; init; }
    }
}

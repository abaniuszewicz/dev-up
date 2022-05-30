namespace DevUp.Api.V1.Controllers.Identity.Responses
{
    public abstract class SucceededResponse
    {
        public string Token { get; init; }
        public string RefreshToken { get; init; }
    }
}

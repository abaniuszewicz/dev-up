namespace DevUp.Api.V1.Controllers.Identity.Requests
{
    public class RefreshUserRequest
    {
        public string Token { get; init; }
        public string RefreshToken { get; init; }
        public DeviceRequest Device { get; init; }
    }
}

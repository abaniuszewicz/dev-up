namespace DevUp.Api.V1.Controllers.Identity.Requests
{
    public class RegisterUserRequest
    {
        /// <example>john-cena</example>
        public string Username { get; init; }
        /// <example>s3cUr3-p4s$</example>
        public string Password { get; init; }
        public DeviceRequest Device { get; init; }
    }
}

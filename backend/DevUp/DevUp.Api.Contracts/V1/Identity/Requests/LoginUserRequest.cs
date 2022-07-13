namespace DevUp.Api.Contracts.V1.Identity.Requests
{
    public class LoginUserRequest
    {
        /// <summary>
        /// Username
        /// </summary>
        /// <example>john-cena</example>
        public string Username { get; init; }
        /// <example>s3cUr3-p4s$</example>
        public string Password { get; init; }
        public DeviceRequest Device { get; init; }
    }
}

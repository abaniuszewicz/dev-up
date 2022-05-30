namespace DevUp.Api.V1.Controllers.Identity.Requests
{
    public class DeviceRequest
    {
        /// <example>b691b7a8-b251-4b11-8034-f3a0a154dffe</example>
        public string Id { get; init; }
        /// <example>iPhone (John)</example>
        public string Name { get; init; }
    }
}

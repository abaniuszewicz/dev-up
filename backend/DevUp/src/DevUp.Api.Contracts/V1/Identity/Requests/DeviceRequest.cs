namespace DevUp.Api.Contracts.V1.Identity.Requests
{
    public class DeviceRequest
    {
        /// <example>b691b7a8-b251-4b11-8034-f3a0a154dffe</example>
        public Guid Id { get; init; }
        /// <example>iPhone (John)</example>
        public string Name { get; init; }
    }
}

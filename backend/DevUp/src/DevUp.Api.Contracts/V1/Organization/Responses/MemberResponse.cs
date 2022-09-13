namespace DevUp.Api.Contracts.V1.Organization.Responses
{
    public class MemberResponse
    {
        /// <example>2cb3ff4a-4b53-4154-91c1-da965e4ecbdb</example>
        public Guid Id { get; }
        /// <example>John Cena</example>
        public string Name { get; }
        /// <example>Senior Software Engineer</example>
        public string Role { get; }
    }
}

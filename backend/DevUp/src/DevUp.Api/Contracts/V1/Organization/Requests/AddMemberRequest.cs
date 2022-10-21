namespace DevUp.Api.Contracts.V1.Organization.Requests
{
    public class AddMemberRequest
    {
        /// <example>Senior Software Engineer</example>
        public string Role { get; }
        /// <example>John Cena</example>
        public string Name { get; }
    }
}

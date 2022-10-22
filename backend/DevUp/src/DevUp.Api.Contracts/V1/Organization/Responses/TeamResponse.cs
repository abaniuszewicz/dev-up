namespace DevUp.Api.Contracts.V1.Organization.Responses
{
    public class TeamResponse
    {
        /// <example>5c480048-36b7-402f-8d19-95c74737282a</example>
        public Guid Id { get; init; }
        /// <example>Somsiady Crew</example>
        public string Name { get; init; }
        public MemberResponse[] Members { get; init; }
    }
}

namespace DevUp.Infrastructure.Postgres.Organization.Dtos
{
    internal sealed class MemberDto
    {
        public Guid TeamId { get; set; }
        public Guid UserId { get; set; }
    }
}

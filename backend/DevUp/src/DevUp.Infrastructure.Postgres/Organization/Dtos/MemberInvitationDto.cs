namespace DevUp.Infrastructure.Postgres.Organization.Dtos
{
    internal sealed class MemberInvitationDto
    {
        public Guid SenderId { get; set; }
        public Guid ReceiverId { get; set; }
        public Guid TeamId { get; set; }
    }
}

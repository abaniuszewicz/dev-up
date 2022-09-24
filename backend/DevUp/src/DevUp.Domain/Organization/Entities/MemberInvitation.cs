using System;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Organization.Entities
{
    public sealed class MemberInvitation : Entity<MemberInvitationId>
    {
        public Guid TeamId { get; init; }
        public MemberInvitation(MemberInvitationId id, Guid teamId) 
            : base(id)
        {
            TeamId = teamId;
        }
    }
}

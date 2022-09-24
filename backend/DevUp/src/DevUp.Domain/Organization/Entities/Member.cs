using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Organization.ValueObjects;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Organization.Entities
{
    public sealed class Member : Entity<MemberId>
    {
        public UserId UserId { get; }
        public Role Role { get; private set; }
        public TeamId TeamId { get; set; }

        public Member(MemberId memberId, UserId userId, Role role) 
            : base(memberId)
        {
            UserId = userId;
        }

        public void ChangeRole(Role role)
        {
            Role = role;
        }
    }
}

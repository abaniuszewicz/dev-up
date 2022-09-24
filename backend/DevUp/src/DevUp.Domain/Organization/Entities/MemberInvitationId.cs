using System;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Organization.Entities
{
    public sealed class MemberInvitationId : EntityId
    {
        public Guid SenderId { get; }
        public Guid ReceiverId { get; }

        public MemberInvitationId(Guid senderId, Guid receiverId)
        {
            SenderId = senderId;
            ReceiverId = receiverId;
        }

        public override bool Equals(EntityId other)
        {
            return other is MemberInvitationId otherMemberInvitationId
                && otherMemberInvitationId.SenderId == SenderId
                && otherMemberInvitationId.ReceiverId == ReceiverId;
        }

        public override int GetHashCode()
        {
            return SenderId.GetHashCode() ^ ReceiverId.GetHashCode();
        }
    }
}

using System;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Organization.Entities
{
    public sealed class TeamId : EntityId
    {
        public Guid Id { get; }

        public TeamId(Guid id)
        {
            Id = id;
        }

        public override string ToString()
        {
            return Id.ToString();
        }

        public override bool Equals(EntityId other)
        {
            return other is TeamId otherTeamId && otherTeamId.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id.GetHashCode();
        }
    }
}

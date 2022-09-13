using System.Collections.Generic;
using DevUp.Domain.Organization.Entities.Exception;
using DevUp.Domain.Organization.ValueObjects;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Organization.Entities
{
    public sealed class Team : Entity<TeamId>
    {
        private HashSet<Member> _members;

        public TeamName Name { get; }
        public IEnumerable<Member> Members { get; }

        public Team(TeamId id, TeamName name) 
            : base(id)
        {
            Name = name;
        }

        public void AddMember(Member member)
        {
            if (_members.Contains(member))
                throw new DuplicateMemberException(Id, member.Id);

            _members.Add(member);
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}

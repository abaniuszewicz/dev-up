using System.Collections.Generic;
using DevUp.Domain.Organization.Entities.Exception;
using DevUp.Domain.Organization.ValueObjects;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Organization.Entities
{
    public sealed class Team : Entity<TeamId>
    {
        private readonly HashSet<Member> _members = new();

        public TeamName Name { get; private set; }
        public IEnumerable<Member> Members { get => _members; }

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

        public void ChangeName(TeamName name)
        {
            Name = name;
        }

        public override string ToString()
        {
            return Name.ToString();
        }
    }
}

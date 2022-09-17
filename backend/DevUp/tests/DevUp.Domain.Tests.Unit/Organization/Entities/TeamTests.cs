using System;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Entities.Exception;
using DevUp.Domain.Organization.ValueObjects;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Organization.Entities
{
    public class TeamTests
    {
        [Test]
        public void Constructor_WhenCalled_AssignsProperties()
        {
            var id = new TeamId(Guid.NewGuid());
            var name = new TeamName("Team");

            var team = new Team(id, name);

            Assert.AreEqual(id, team.Id);
            Assert.AreEqual(name, team.Name);
        }

        [Test]
        public void AddMember_ForEveryUniqueMember_AddsMemberToTheMemberList()
        {
            var team = new Team(new(Guid.NewGuid()), new("Team"));
            var member1 = new Member(new(Guid.NewGuid()), new(Guid.NewGuid()), new Role("role"));
            var member2 = new Member(new(Guid.NewGuid()), new(Guid.NewGuid()), new Role("role"));
            var member3 = new Member(new(Guid.NewGuid()), new(Guid.NewGuid()), new Role("role"));

            team.AddMember(member1);
            team.AddMember(member2);
            team.AddMember(member3);

            Assert.That(team.Members, Has.Count.EqualTo(3));
            Assert.That(team.Members, Is.EquivalentTo(new[] { member1, member2, member3 }));
        }

        [Test]
        public void AddMember_WhenTryingToAddDuplicateMember_ThrowsDuplicateMemberException()
        {
            var team = new Team(new(Guid.NewGuid()), new("Team"));
            var guid = Guid.NewGuid();
            var member = new Member(new(guid), new(Guid.NewGuid()), new("role"));
            team.AddMember(member);
            var duplicateMember = new Member(new(guid), new(Guid.NewGuid()), new("role"));

            var exception = Assert.Throws<DuplicateMemberException>(() => team.AddMember(duplicateMember));
        }

        [Test]
        public void ChangeName_WhenCalled_UpdatesName()
        {
            var team = new Team(new(Guid.NewGuid()), new("Team"));
            var newName = new TeamName("New name");

            team.ChangeName(newName);

            Assert.AreEqual(team.Name, newName);
        }
    }
}

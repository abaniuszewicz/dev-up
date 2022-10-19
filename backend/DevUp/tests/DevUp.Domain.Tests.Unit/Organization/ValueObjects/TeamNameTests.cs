using DevUp.Domain.Organization.ValueObjects;
using DevUp.Domain.Organization.ValueObjects.Exceptions;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Organization.ValueObjects
{
    public class TeamNameTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_WhenGivenEmptyTeamName_ThrowsEmptyTeamNameException(string teamName)
        {
            var exception = Assert.Throws<EmptyTeamNameException>(() => new TeamName(teamName));
        }

        [Test]
        public void Constructor_WhenGivenValidTeamName_AssignsValueProperty()
        {
            const string value = "team";
            var teamName = new TeamName(value);
            Assert.AreEqual(teamName.Value, value);
        }

        [Test]
        public void Equality_WhenCompared_ChecksByValue()
        {
            var teamName1 = new TeamName("team");
            var teamName2 = new TeamName("team");

            Assert.AreEqual(teamName1, teamName2);
        }
    }
}

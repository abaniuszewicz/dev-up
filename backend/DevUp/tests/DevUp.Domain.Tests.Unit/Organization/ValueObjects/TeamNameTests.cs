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
            const string value = "Team";
            var teamName = new TeamName(value);
            Assert.AreEqual(teamName.Value, value);
        }
    }
}

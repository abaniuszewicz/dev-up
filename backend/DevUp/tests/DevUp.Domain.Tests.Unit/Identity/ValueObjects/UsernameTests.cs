using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class UsernameTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Constructor_WhenGivenEmptyUsername_ThrowsEmptyUsernameException(string username)
        {
            var exception = Assert.Throws<EmptyUsernameException>(() => new Username(username));
        }

        [Test]
        public void Constructor_WhenGivenValidUsername_AssignsValueProperty()
        {
            const string value = "valid-username";
            var username = new Username(value);
            Assert.AreEqual(username.Value, value);
        }

        [Test]
        public void Equality_WhenCompared_ChecksByValue()
        {
            var username1 = new Username("valid-username");
            var username2 = new Username("valid-username");

            Assert.AreEqual(username1, username2);
        }
    }
}

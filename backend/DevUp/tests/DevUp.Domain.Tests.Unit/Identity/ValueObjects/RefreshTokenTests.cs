using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class RefreshTokenTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Constructor_WhenGivenEmptyToken_ThrowsEmptyRefreshTokenException(string refreshToken)
        {
            Assert.Throws<EmptyRefreshTokenException>(() => new RefreshToken(refreshToken));
        }

        [Test]
        public void Constructor_WhenGivenValidToken_AssignsValueProperty()
        {
            const string value = "r4nd0mha$h";
            var refreshToken = new RefreshToken(value);
            Assert.AreEqual(refreshToken.Value, value);
        }

        [Test]
        public void Equality_WhenCompared_ChecksByValue()
        {
            var refreshToken1 = new RefreshToken("r4nd0mha$h");
            var refreshToken2 = new RefreshToken("r4nd0mha$h");

            Assert.AreEqual(refreshToken1, refreshToken2);
        }
    }
}

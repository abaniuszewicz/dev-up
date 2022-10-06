using System.Collections.Generic;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class RefreshTokenTests
    {
        private class Dummy : RefreshToken
        {
            public Dummy(string refreshToken) 
                : base(refreshToken)
            {
            }

            public new IEnumerable<object> GetEqualityComponents() => base.GetEqualityComponents();
        }

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
        public void GetEqualityComponents_WhenCalled_ReturnsTokenValue()
        {
            const string value = "r4nd0mha$h";
            var refreshToken = new Dummy(value);

            var result = refreshToken.GetEqualityComponents();

            Assert.That(result, Has.One.EqualTo(value));
        }
    }
}

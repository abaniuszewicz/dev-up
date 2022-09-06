using System.Collections.Generic;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class TokenTests
    {
        private class Dummy : Token
        {
            public Dummy(string token) : base(token)
            {
            }

            public new IEnumerable<object> GetEqualityComponents() => base.GetEqualityComponents();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Constructor_WhenGivenEmptyToken_ThrowsEmptyTokenException(string token)
        {
            Assert.Throws<EmptyTokenException>(() => new Token(token));
        }

        [Test]
        [TestCase("jwtmustcontain3sections")]
        [TestCase("jwtmustcont.ain3sections")]
        [TestCase("jwtmu.stcont.ain3sec.tions")]
        public void Constructor_WhenGivenTokenWithInvalidFormat_ThrowsInvalidTokenFormatException(string token)
        {
            Assert.Throws<InvalidTokenFormatException>(() => new Token(token));
        }

        [Test]
        public void Constructor_WhenGivenValidToken_AssignsValueProperty()
        {
            const string value = "header.payload.signature";
            var token = new Token(value);
            Assert.AreEqual(token.Value, value);
        }

        [Test]
        public void GetEqualityComponents_WhenCalled_ReturnsTokenValue()
        {
            const string value = "header.payload.signature";
            var token = new Dummy(value);

            var result = token.GetEqualityComponents();

            Assert.That(result, Has.One.EqualTo(value));
        }
    }
}

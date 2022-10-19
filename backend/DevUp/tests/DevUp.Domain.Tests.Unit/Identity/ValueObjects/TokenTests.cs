using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class TokenTests
    {
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
        public void Equality_WhenCompared_ChecksByValue()
        {
            var token1 = new Token("header.payload.signature");
            var token2 = new Token("header.payload.signature");

            Assert.AreEqual(token1, token2);
        }
    }
}

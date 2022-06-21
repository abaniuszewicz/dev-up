using System.Collections.Generic;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.ValueObjects;
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
        [TestCase(null, TokenValidationException.NullMessage)]
        [TestCase("", TokenValidationException.EmptyMessage)]
        [TestCase(" ", TokenValidationException.EmptyMessage)]
        public void Constructor_WhenGivenInvalidToken_ThrowsTokenValidationExeptionWithErrorDescription(string token, string error)
        {
            var exception = Assert.Throws<TokenValidationException>(() => new Token(token));
            Assert.That(exception!.Errors, Has.One.EqualTo(error));
        }

        [Test]
        public void Constructor_WhenGivenValidToken_AssignsValueProperty()
        {
            const string value = "valid.token";
            var token = new Token(value);
            Assert.AreEqual(token.Value, value);
        }

        [Test]
        public void GetEqualityComponents_WhenCalled_ReturnsTokenValue()
        {
            const string value = "valid.token";
            var token = new Dummy(value);

            var result = token.GetEqualityComponents();

            Assert.That(result, Has.One.EqualTo(value));
        }
    }
}

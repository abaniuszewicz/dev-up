using System.Collections.Generic;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class UsernameTests
    {
        private class Dummy : Username
        {
            public Dummy(string username) : base(username)
            {
            }

            public new IEnumerable<object> GetEqualityComponents() => base.GetEqualityComponents();
        }

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
        public void GetEqualityComponents_WhenCalled_ReturnsUsernameValue()
        {
            const string value = "valid-username";
            var username = new Dummy(value);

            var result = username.GetEqualityComponents();

            Assert.That(result, Has.One.EqualTo(value));
        }
    }
}

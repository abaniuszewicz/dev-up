using System.Collections.Generic;
using DevUp.Domain.Identity.ValueObjects;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class PasswordTests
    {
        private class Dummy : Password
        {
            public Dummy(string password) : base(password)
            {
            }

            public new IEnumerable<object> GetEqualityComponents() => base.GetEqualityComponents();
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_WhenGivenEmptyPassword_ThrowsEmptyPasswordException(string password)
        {
            var exception = Assert.Throws<EmptyPasswordException>(() => new Password(password));
        }

        [Test]
        public void Constructor_WhenGivenValidPassword_AssignsValueProperty()
        {
            const string value = "lowercaseUPPERCASE#1";
            var password = new Password(value);
            Assert.AreEqual(password.Value, value);
        }

        [Test]
        public void GetEqualityComponents_WhenCalled_ReturnsPasswordValue()
        {
            const string value = "lowercaseUPPERCASE#1";
            var password = new Dummy(value);

            var result = password.GetEqualityComponents();

            Assert.That(result, Has.One.EqualTo(value));
        }
    }
}

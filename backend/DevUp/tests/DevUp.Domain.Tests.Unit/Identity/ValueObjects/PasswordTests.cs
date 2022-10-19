using DevUp.Domain.Identity.ValueObjects;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class PasswordTests
    {
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
            const string value = "password";
            var password = new Password(value);
            Assert.AreEqual(password.Value, value);
        }

        [Test]
        public void Equality_WhenCompared_ChecksByValue()
        {
            var password1 = new Password("password");
            var password2 = new Password("password");

            Assert.AreEqual(password1, password2);
        }
    }
}

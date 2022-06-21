using System.Collections.Generic;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.ValueObjects;
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
        [TestCase(null, UsernameValidationException.NullMessage)]
        [TestCase("short", UsernameValidationException.InvalidLengthMessage)]
        [TestCase("too-long-to-be-considered-a-good-username", UsernameValidationException.InvalidLengthMessage)]
        [TestCase("wrong-char$acter", UsernameValidationException.InvalidCharactersMessage)]
        [TestCase("wrong-char0acter", UsernameValidationException.InvalidCharactersMessage)]
        [TestCase("wrong-char_acter", UsernameValidationException.InvalidCharactersMessage)]
        [TestCase("WRONG-CHAR字ACTER", UsernameValidationException.InvalidCharactersMessage)]
        [TestCase("WRONG-CHARACTER", UsernameValidationException.InvalidCharactersMessage)]
        [TestCase("-wrong-start", UsernameValidationException.InvalidFirstOrLastCharacterMessage)]
        [TestCase("wrong-end-", UsernameValidationException.InvalidFirstOrLastCharacterMessage)]
        public void Constructor_WhenGivenInvalidUsername_ThrowsUsernameValidationExeptionWithErrorDescription(string username, string error)
        {
            var exception = Assert.Throws<UsernameValidationException>(() => new Username(username));
            Assert.That(exception!.Errors, Has.One.EqualTo(error));
        }

        [Test]
        [TestCase("-$-", new[] { UsernameValidationException.InvalidLengthMessage, UsernameValidationException.InvalidCharactersMessage, UsernameValidationException.InvalidFirstOrLastCharacterMessage })]
        [TestCase("-", new[] { UsernameValidationException.InvalidLengthMessage, UsernameValidationException.InvalidFirstOrLastCharacterMessage })]
        public void Constructor_WhenMultipleConstraintsFail_ThrowsExceptionWithAllErrors(string username, string[] errors)
        {
            var exception = Assert.Throws<UsernameValidationException>(() => new Username(username));
            CollectionAssert.AreEquivalent(exception!.Errors, errors);
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

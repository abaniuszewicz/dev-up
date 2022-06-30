using System.Collections.Generic;
using DevUp.Domain.Identity.Exceptions;
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
        [TestCase(null, PasswordValidationException.NullMessage)]
        [TestCase("lU#1", PasswordValidationException.TooShortMessage)]
        [TestCase("UPPERCASE#1", PasswordValidationException.NoLowercaseLetterMessage)]
        [TestCase("lowercase#1", PasswordValidationException.NoUppercaseLetterMessage)]
        [TestCase("lowercaseUPPERCASE1", PasswordValidationException.NoSpecialCharacterMessage)]
        [TestCase("lowercaseUPPERCASE#", PasswordValidationException.NoDigitMessage)]
        public void Constructor_WhenGivenInvalidPassword_ThrowsPasswordValidationExeption(string password, string error)
        {
            var exception = Assert.Throws<PasswordValidationException>(() => new Password(password));
            Assert.That(exception!.Errors, Has.One.EqualTo(error));
        }

        [Test]
        [TestCase("lowercase#", new[] { PasswordValidationException.NoUppercaseLetterMessage, PasswordValidationException.NoDigitMessage })]
        [TestCase("", new[] { PasswordValidationException.TooShortMessage, PasswordValidationException.NoLowercaseLetterMessage, PasswordValidationException.NoUppercaseLetterMessage, PasswordValidationException.NoSpecialCharacterMessage, PasswordValidationException.NoDigitMessage })]
        public void Constructor_WhenMultipleConstraintsFail_ThrowsExceptionWithAllErrors(string password, string[] errors)
        {
            var exception = Assert.Throws<PasswordValidationException>(() => new Password(password));
            CollectionAssert.AreEquivalent(exception!.Errors, errors);
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

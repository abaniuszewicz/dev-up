using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.ValueObjects;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class PasswordTests
    {
        [Test]
        [TestCase(null, PasswordValidationException.NullMessage)]
        [TestCase("lU#1", PasswordValidationException.TooShortMessage)]
        [TestCase("UPPERCASE#1", PasswordValidationException.NoLowercaseLetterMessage)]
        [TestCase("lowercase#1", PasswordValidationException.NoUppercaseLetterMessage)]
        [TestCase("lowercaseUPPERCASE1", PasswordValidationException.NoSpecialCharacterMessage)]
        [TestCase("lowercaseUPPERCASE#", PasswordValidationException.NoDigitMessage)]
        public void Constructor_WhenGivenInvalidPassword_ThrowsPasswordValidationExeptionWithErrorDescription(string password, string error)
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
            CollectionAssert.AreEqual(exception!.Errors, errors);
        }

        [Test]
        public void Constructor_WhenGivenValidPassword_AssignsValueProperty()
        {
            const string value = "lowercaseUPPERCASE#1";
            var password = new Password(value);
            Assert.AreEqual(password.Value, value);
        }

        [Test]
        public void Equality_WhenCompared_ChecksByValue()
        {
            var password1 = new Password("lowercaseUPPERCASE#1");
            var password2 = new Password("lowercaseUPPERCASE#1");

            Assert.IsTrue(object.Equals(password1, password2));
            Assert.IsTrue(password1.Equals(password2));
            Assert.IsTrue(password1 == password2);
        }
    }
}

using DevUp.Domain.Identity.ValueObjects;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class PasswordHashTests
    {
        [Test]
        public void Constructor_WhenGivenValidPasswordHash_AssignsValueProperty()
        {
            const string value = "r4nd0m-h4$h";
            var hash = new PasswordHash(value);
            Assert.AreEqual(hash.Value, value);
        }

        [Test]
        public void Equality_WhenCompared_ChecksByValue()
        {
            var hash1 = new PasswordHash("r4nd0m-h4$h");
            var hash2 = new PasswordHash("r4nd0m-h4$h");

            Assert.AreEqual(hash1, hash2);
        }
    }
}

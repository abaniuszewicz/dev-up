using System.Collections.Generic;
using DevUp.Domain.Identity.ValueObjects;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class PasswordHashTests
    {
        private class Dummy : PasswordHash
        {
            public Dummy(string passwordHash) : base(passwordHash)
            {
            }

            public new IEnumerable<object> GetEqualityComponents() => base.GetEqualityComponents();
        }

        [Test]
        public void GetEqualityComponents_WhenCalled_ReturnsHashValue()
        {
            const string value = "r4nd0m-h4$h";
            var hash = new Dummy(value);

            var result = hash.GetEqualityComponents();

            Assert.That(result, Has.One.EqualTo(value));
        }
    }
}

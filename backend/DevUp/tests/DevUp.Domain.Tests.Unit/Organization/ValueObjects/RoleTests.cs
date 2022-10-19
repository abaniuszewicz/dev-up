using DevUp.Domain.Organization.ValueObjects;
using DevUp.Domain.Organization.ValueObjects.Exceptions;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Organization.ValueObjects
{
    public class RoleTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_WhenGivenEmptyRole_ThrowsEmptyRoleException(string role)
        {
            var exception = Assert.Throws<EmptyRoleException>(() => new Role(role));
        }

        [Test]
        public void Constructor_WhenGivenValidRole_AssignsValueProperty()
        {
            const string value = "role";
            var role = new Role(value);
            Assert.AreEqual(role.Value, value);
        }

        [Test]
        public void Equality_WhenCompared_ChecksByValue()
        {
            var role1 = new Role("role");
            var role2 = new Role("role");

            Assert.AreEqual(role1, role2);
        }
    }
}

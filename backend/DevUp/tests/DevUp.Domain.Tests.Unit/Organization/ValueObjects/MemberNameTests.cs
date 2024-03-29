﻿using DevUp.Domain.Organization.ValueObjects;
using DevUp.Domain.Organization.ValueObjects.Exceptions;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Organization.ValueObjects
{
    public class MemberNameTests
    {
        [Test]
        [TestCase(null)]
        [TestCase("")]
        public void Constructor_WhenGivenEmptyMemberName_ThrowsEmptyMemberNameException(string memberName)
        {
            var exception = Assert.Throws<EmptyMemberNameException>(() => new MemberName(memberName));
        }

        [Test]
        public void Constructor_WhenGivenValidMemberName_AssignsValueProperty()
        {
            const string value = "member";
            var memberName = new MemberName(value);
            Assert.AreEqual(memberName.Value, value);
        }

        [Test]
        public void Equality_WhenCompared_ChecksByValue()
        {
            var memberName1 = new MemberName("member");
            var memberName2 = new MemberName("member");

            Assert.AreEqual(memberName1, memberName2);
        }
    }
}

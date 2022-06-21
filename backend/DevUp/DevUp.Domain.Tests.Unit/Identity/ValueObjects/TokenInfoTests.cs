using System;
using System.Collections.Generic;
using DevUp.Common;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;
using Moq;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class TokenInfoTests
    {
        private class Dummy : TokenInfo
        {
            public Dummy(Token token, string jti, UserId userId, DateTimeRange lifespan) : base(token, jti, userId, lifespan)
            {
            }

            public new IEnumerable<object> GetEqualityComponents() => base.GetEqualityComponents();
        }

        private static readonly DateTime FirstJan2000_8_00 = new DateTime(2000, 1, 1, 8, 0, 0);
        private static readonly DateTime FirstJan2000_8_15 = new DateTime(2000, 1, 1, 8, 15, 0);

        private static readonly DateTime[] DatesWithinLifespan = new[] { FirstJan2000_8_00, FirstJan2000_8_00.AddMinutes(7).AddSeconds(30), FirstJan2000_8_15 };
        private static readonly DateTime[] DatesOutsideLifespan = new[] { FirstJan2000_8_00.AddMinutes(-1), FirstJan2000_8_15.AddMinutes(1) };

        private Token _token;
        private string _jti;
        private UserId _userId;
        private DateTimeRange _lifespan;
        private TokenInfo _tokenInfo;

        [SetUp]
        public void SetUp()
        {
            _token = new Token("valid.token");
            _jti = "jti";
            _userId = new UserId(Guid.Parse("85f7c00f-47c3-4079-bb5b-7a3e08fb2309"));
            _lifespan = new DateTimeRange(start: FirstJan2000_8_00, end: FirstJan2000_8_15);
            _tokenInfo = new TokenInfo(_token, _jti, _userId, _lifespan);
        }

        [Test]
        public void Constructor_WhenCalled_AssignsProperties()
        {
            Assert.AreEqual(_tokenInfo.Token, _token);
            Assert.AreEqual(_tokenInfo.Jti, _jti);
            Assert.AreEqual(_tokenInfo.UserId, _userId);
            Assert.AreEqual(_tokenInfo.Lifespan, _lifespan);
        }

        [Test]
        [TestCaseSource(nameof(DatesWithinLifespan))]
        public void IsActive_ForDatesWithinLifespan_ReturnsTrue(DateTime date)
        {
            var dateProvider = new Mock<IDateTimeProvider>();
            dateProvider.Setup(dp => dp.UtcNow).Returns(date);

            Assert.IsTrue(_tokenInfo.IsActive(dateProvider.Object));
        }

        [Test]
        [TestCaseSource(nameof(DatesOutsideLifespan))]
        public void IsActive_ForDatesOutsideLifespan_ReturnsFalse(DateTime date)
        {
            var dateProvider = new Mock<IDateTimeProvider>();
            dateProvider.Setup(dp => dp.UtcNow).Returns(date);

            Assert.IsFalse(_tokenInfo.IsActive(dateProvider.Object));
        }

        [Test]
        public void GetEqualityComponents_WhenCalled_ReturnsTokenValue()
        {
            var tokenInfo = new Dummy(_token, _jti, _userId, _lifespan);

            var result = tokenInfo.GetEqualityComponents();

            Assert.That(result, Has.One.EqualTo(_token));
        }
    }
}

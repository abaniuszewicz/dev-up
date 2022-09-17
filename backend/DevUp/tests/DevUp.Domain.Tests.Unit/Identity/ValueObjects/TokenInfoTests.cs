using System.Collections.Generic;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
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

        private IdentityFaker _faker;

        [SetUp]
        public void SetUp()
        {
            _faker = new IdentityFaker();
        }

        [Test]
        public void Constructor_WhenCalled_AssignsProperties()
        {
            var tokenInfo = new TokenInfo(_faker.TokenInfo.Token, _faker.TokenInfo.Jti, _faker.TokenInfo.UserId, _faker.TokenInfo.Lifespan);

            Assert.AreEqual(tokenInfo.Token, _faker.TokenInfo.Token);
            Assert.AreEqual(tokenInfo.Jti, _faker.TokenInfo.Jti);
            Assert.AreEqual(tokenInfo.UserId, _faker.TokenInfo.UserId);
            Assert.AreEqual(tokenInfo.Lifespan, _faker.TokenInfo.Lifespan);
        }

        [Test]
        public void IsActive_ForDatesWithinLifespan_ReturnsTrue()
        {
            var dates = new[]
            {
                _faker.TokenInfo.Lifespan.Start,
                _faker.Faker.Date.Between(_faker.TokenInfo.Lifespan.Start, _faker.TokenInfo.Lifespan.End),
                _faker.TokenInfo.Lifespan.End
            };

            foreach (var date in dates)
            {
                var dateProvider = new Mock<IDateTimeProvider>();
                dateProvider.Setup(dp => dp.Now).Returns(date);

                Assert.IsTrue(_faker.TokenInfo.IsActive(dateProvider.Object));
            }
        }

        [Test]
        public void IsActive_ForDatesOutsideLifespan_ReturnsFalse()
        {
            var dates = new[]
            {
                _faker.Faker.Date.Recent(1, _faker.TokenInfo.Lifespan.Start),
                _faker.Faker.Date.Soon(1, _faker.TokenInfo.Lifespan.End)
            };

            foreach (var date in dates)
            {
                var dateProvider = new Mock<IDateTimeProvider>();
                dateProvider.Setup(dp => dp.Now).Returns(date);

                Assert.IsFalse(_faker.TokenInfo.IsActive(dateProvider.Object));
            }
        }

        [Test]
        public void GetEqualityComponents_WhenCalled_ReturnsTokenValue()
        {
            var tokenInfo = new Dummy(_faker.TokenInfo.Token, _faker.TokenInfo.Jti, _faker.TokenInfo.UserId, _faker.TokenInfo.Lifespan);

            var result = tokenInfo.GetEqualityComponents();

            Assert.That(result, Has.One.EqualTo(_faker.TokenInfo.Token));
        }
    }
}

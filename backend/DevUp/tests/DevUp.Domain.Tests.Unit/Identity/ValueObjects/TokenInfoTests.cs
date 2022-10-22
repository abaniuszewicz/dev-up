using DevUp.Domain.Common.Services;
using DevUp.Domain.Identity.ValueObjects;
using Moq;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class TokenInfoTests
    {
        private IdentityFaker _faker;

        [SetUp]
        public void SetUp()
        {
            _faker = new IdentityFaker();
        }

        [Test]
        public void Constructor_WhenCalled_AssignsProperties()
        {
            var tokenInfo = new TokenInfo(_faker.TokenInfo.Token, _faker.TokenInfo.Jti, _faker.TokenInfo.UserId, _faker.TokenInfo.DeviceId, _faker.TokenInfo.Lifespan);

            Assert.AreEqual(tokenInfo.Token, _faker.TokenInfo.Token);
            Assert.AreEqual(tokenInfo.Jti, _faker.TokenInfo.Jti);
            Assert.AreEqual(tokenInfo.UserId, _faker.TokenInfo.UserId);
            Assert.AreEqual(tokenInfo.DeviceId, _faker.TokenInfo.DeviceId);
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
        public void Equality_WhenCompared_ChecksByTokenValue()
        {
            var differentFaker = new IdentityFaker();
            var tokenInfo1 = new TokenInfo(_faker.TokenInfo.Token, _faker.TokenInfo.Jti, _faker.TokenInfo.UserId, _faker.TokenInfo.DeviceId, _faker.TokenInfo.Lifespan);
            var tokenInfo2 = new TokenInfo(_faker.TokenInfo.Token, differentFaker.TokenInfo.Jti, differentFaker.TokenInfo.UserId, differentFaker.TokenInfo.DeviceId, differentFaker.TokenInfo.Lifespan);

            Assert.AreEqual(tokenInfo1, tokenInfo2);
        }
    }
}

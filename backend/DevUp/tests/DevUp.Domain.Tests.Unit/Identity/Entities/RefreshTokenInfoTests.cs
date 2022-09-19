using Bogus.DataSets;
using DevUp.Domain.Common.Services;
using Moq;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.Entities
{
    public class RefreshTokenInfoTests
    {
        private IdentityFaker _faker;

        [SetUp]
        public void SetUp()
        {
            _faker = new IdentityFaker();
        }

        [Test]
        public void BelongsTo_ForUserWithTheSameId_ReturnsTrue()
        {
            var rti = _faker.RefreshTokenInfo;
            var user = _faker.User;

            Assert.IsTrue(rti.BelongsTo(user));
        }

        [Test]
        public void BelongsTo_ForUserWithDifferentId_ReturnsFalse()
        {
            var rti = _faker.RefreshTokenInfo;
            var differentUser = new IdentityFaker().User;

            Assert.IsFalse(rti.BelongsTo(differentUser));
        }

        [Test]
        public void BelongsTo_ForTokenInfoWithTheSameJti_ReturnsTrue()
        {
            var rti = _faker.RefreshTokenInfo;
            var ti = _faker.TokenInfo;

            Assert.IsTrue(rti.BelongsTo(ti));
        }

        [Test]
        public void BelongsTo_ForTokenInfoWithDifferentJti_ReturnsFalse()
        {
            var rti = _faker.RefreshTokenInfo;
            var differentTi = new IdentityFaker().TokenInfo;

            Assert.IsFalse(rti.BelongsTo(differentTi));
        }

        [Test]
        public void BelongsTo_ForDeviceWithTheSameId_ReturnsTrue()
        {
            var rti = _faker.RefreshTokenInfo;
            var device = _faker.Device;

            Assert.IsTrue(rti.BelongsTo(device));
        }

        [Test]
        public void BelongsTo_ForDeviceWithDifferentId_ReturnsFalse()
        {
            var rti = _faker.RefreshTokenInfo;
            var differentDevice = new IdentityFaker().Device;

            Assert.IsFalse(rti.BelongsTo(differentDevice));
        }

        [Test]
        public void IsActive_ForDatesWithinLifespan_ReturnsTrue()
        {
            var rti = _faker.RefreshTokenInfo;
            var dateProvider = new Mock<IDateTimeProvider>();
            var dates = new[] { rti.Lifespan.Start, new Date().Between(rti.Lifespan.Start, rti.Lifespan.End), rti.Lifespan.End };

            foreach (var date in dates)
            {
                dateProvider.Setup(dp => dp.Now).Returns(date);
                Assert.IsTrue(rti.IsActive(dateProvider.Object));
            }
        }

        [Test]
        public void IsActive_ForDatesOutsideLifespan_ReturnsFalse()
        {
            var rti = _faker.RefreshTokenInfo;
            var dateProvider = new Mock<IDateTimeProvider>();
            var dates = new[] { new Date().Recent(1, rti.Lifespan.Start), new Date().Soon(1, rti.Lifespan.End) };

            foreach (var date in dates)
            {
                dateProvider.Setup(dp => dp.Now).Returns(date);
                Assert.IsFalse(rti.IsActive(dateProvider.Object));
            }
        }
    }
}

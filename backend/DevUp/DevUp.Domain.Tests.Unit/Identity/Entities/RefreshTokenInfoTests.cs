using System;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;
using Moq;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.Entities
{
    public class RefreshTokenInfoTests
    {
        private RefreshToken _refreshToken;
        private string _jti;
        private UserId _userId;
        private DeviceId _deviceId;
        private DateTimeRange _lifespan;
        private RefreshTokenInfo _refreshTokenInfo;

        private static readonly DateTime FirstJan2000_8_00 = new DateTime(2000, 1, 1, 8, 0, 0);
        private static readonly DateTime FirstJan2000_8_15 = new DateTime(2000, 1, 1, 8, 15, 0);

        private static readonly DateTime[] DatesWithinLifespan = new[] { FirstJan2000_8_00, FirstJan2000_8_00.AddMinutes(7).AddSeconds(30), FirstJan2000_8_15 };
        private static readonly DateTime[] DatesOutsideLifespan = new[] { FirstJan2000_8_00.AddMinutes(-1), FirstJan2000_8_15.AddMinutes(1) };

        [SetUp]
        public void SetUp()
        {
            _refreshToken = new RefreshToken("--valid_refresh_token--");
            _jti = "jti";
            _userId = new UserId(Guid.Parse("85f7c00f-47c3-4079-bb5b-7a3e08fb2309"));
            _deviceId = new DeviceId("123-321");
            _lifespan = new DateTimeRange(start: FirstJan2000_8_00, end: FirstJan2000_8_15);

            _refreshTokenInfo = new RefreshTokenInfo(_refreshToken, _jti, _userId, _deviceId, _lifespan);
        }

        [Test]
        public void BelongsTo_ForUserWithTheSameId_ReturnsTrue()
        {
            var sameUser = new User(new UserId(Guid.Parse("85f7c00f-47c3-4079-bb5b-7a3e08fb2309")), new Username("name-is-not-important"));
            Assert.IsTrue(_refreshTokenInfo.BelongsTo(sameUser));
        }

        [Test]
        public void BelongsTo_ForUserWithDifferentId_ReturnsFalse()
        {
            var differentUser = new User(new UserId(Guid.Parse("99999999-47c3-4079-bb5b-7a3e08fb2309")), new Username("name-is-not-important"));
            Assert.IsFalse(_refreshTokenInfo.BelongsTo(differentUser));
        }

        [Test]
        public void BelongsTo_ForTokenInfoWithTheSameJti_ReturnsTrue()
        {
            var sameJtiTokenInfo = new TokenInfo(new Token("valid.token"), _jti, _userId, _lifespan);
            Assert.IsTrue(_refreshTokenInfo.BelongsTo(sameJtiTokenInfo));
        }

        [Test]
        public void BelongsTo_ForTokenInfoWithDifferentJti_ReturnsFalse()
        {
            var differentJtiTokenInfo = new TokenInfo(new Token("valid.token"), "different_jti", _userId, _lifespan);
            Assert.IsFalse(_refreshTokenInfo.BelongsTo(differentJtiTokenInfo));
        }

        [Test]
        public void BelongsTo_ForDeviceWithTheSameId_ReturnsTrue()
        {
            var sameDevice = new Device(new DeviceId("123-321"), "name is not important");
            Assert.IsTrue(_refreshTokenInfo.BelongsTo(sameDevice));
        }

        [Test]
        public void BelongsTo_ForDeviceWithDifferentId_ReturnsFalse()
        {
            var differentDevice = new Device(new DeviceId("999-999"), "name is not important");
            Assert.IsFalse(_refreshTokenInfo.BelongsTo(differentDevice));
        }

        [Test]
        [TestCaseSource(nameof(DatesWithinLifespan))]
        public void IsActive_ForDatesWithinLifespan_ReturnsTrue(DateTime date)
        {
            var dateProvider = new Mock<IDateTimeProvider>();
            dateProvider.Setup(dp => dp.Now).Returns(date);

            Assert.IsTrue(_refreshTokenInfo.IsActive(dateProvider.Object));
        }

        [Test]
        [TestCaseSource(nameof(DatesOutsideLifespan))]
        public void IsActive_ForDatesOutsideLifespan_ReturnsFalse(DateTime date)
        {
            var dateProvider = new Mock<IDateTimeProvider>();
            dateProvider.Setup(dp => dp.Now).Returns(date);

            Assert.IsFalse(_refreshTokenInfo.IsActive(dateProvider.Object));
        }
    }
}

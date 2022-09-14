using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.ValueObjects;
using Microsoft.IdentityModel.Tokens;
using Moq;
using NUnit.Framework;

using static DevUp.Tests.Utilities.ObjectMothers.Identity.IdentityObjectMother;

namespace DevUp.Domain.Tests.Unit.Identity.Services
{
    public class TokenServiceTests
    {
        private TokenInfo TokenInfo;
        private RefreshTokenInfo RefreshTokenInfo;

        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private Mock<IDeviceRepository> _deviceRepositoryMock;
        private Mock<IDateTimeProvider> _dateProviderMock;
        private Mock<IJwtSettings> _jwtSettingsMock;

        private ITokenService _tokenService;

        [SetUp]
        public void SetUp()
        {
            TokenInfo = new TokenInfo(SerenaWilliams.Token, "fake-jti",
                SerenaWilliams.UserId, new(new(2000, 1, 1), new(2000, 1, 3)));
            RefreshTokenInfo = new RefreshTokenInfo(SerenaWilliams.RefreshToken, 
                "fake-jti", SerenaWilliams.UserId, SerenaWilliams.DeviceId, new(new(2000, 1, 1), new(2000, 1, 5)));

            _userRepositoryMock = new Mock<IUserRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _deviceRepositoryMock = new Mock<IDeviceRepository>();
            _dateProviderMock = new Mock<IDateTimeProvider>();
            _jwtSettingsMock = new Mock<IJwtSettings>();

            _tokenService = new TokenService(_userRepositoryMock.Object, _refreshTokenRepositoryMock.Object, _deviceRepositoryMock.Object, _dateProviderMock.Object, _jwtSettingsMock.Object);
        }

        [Test]
        public async Task CreateAsync_WhenCalled_StoresDeviceAndRefreshTokenInRepository()
        {
            var jan1st_15_0_0 = new DateTime(2000, 1, 1, 15, 0, 0);
            _dateProviderMock.Setup(dp => dp.Now).Returns(jan1st_15_0_0);
            _jwtSettingsMock.Setup(jwts => jwts.JwtExpiryMs).Returns(100);
            _jwtSettingsMock.Setup(jwts => jwts.JwtRefreshExpiryMs).Returns(300);
            _jwtSettingsMock.Setup(jwts => jwts.Secret).Returns(Encoding.ASCII.GetBytes("random-30-char-long-secret-key"));
            _jwtSettingsMock.Setup(jwts => jwts.TokenValidationParameters).Returns(new TokenValidationParameters() 
            { 
                IssuerSigningKey = new SymmetricSecurityKey(_jwtSettingsMock.Object.Secret) 
            });

            await _tokenService.CreateAsync(JohnCena.User, JohnCena.Device, CancellationToken.None);

            _deviceRepositoryMock.Verify(dr => dr.AddAsync(JohnCena.Device, It.IsAny<CancellationToken>()), Times.Once);

            var expectedRefreshTokenLifetime = new DateTimeRange(jan1st_15_0_0, jan1st_15_0_0.AddMilliseconds(_jwtSettingsMock.Object.JwtRefreshExpiryMs));
            _refreshTokenRepositoryMock.Verify(rtr => rtr.AddAsync(
                It.Is<RefreshTokenInfo>(rti => rti.Lifespan == expectedRefreshTokenLifetime && rti.UserId == JohnCena.UserId && rti.DeviceId == JohnCena.DeviceId), 
                It.IsAny<CancellationToken>())
            , Times.Once);
        }

        [Test]
        public async Task DescribeAsync_WhenCalledWithRefreshToken_FetchesDescribedValueFromRepository()
        {
            await _tokenService.DescribeAsync(JohnCena.RefreshToken, CancellationToken.None);
            _refreshTokenRepositoryMock.Verify(rtr => rtr.GetByIdAsync(JohnCena.RefreshToken, CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task DescribeAsync_WhenCalledWithRefreshTokenThatExistsInRepository_ReturnsDescribedValue()
        {
            _refreshTokenRepositoryMock.Setup(rtr => rtr.GetByIdAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(RefreshTokenInfo);

            var result = await _tokenService.DescribeAsync(SerenaWilliams.RefreshToken, CancellationToken.None);

            Assert.AreEqual(RefreshTokenInfo, result);
        }

        [Test]
        public async Task DescribeAsync_WhenCalledWithRefreshTokenThatDoesNotExistInRepository_ReturnsNull()
        {
            RefreshTokenInfo? doesNotExist = null;
            _refreshTokenRepositoryMock.Setup(rtr => rtr.GetByIdAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(doesNotExist);

            var result = await _tokenService.DescribeAsync(SerenaWilliams.RefreshToken, CancellationToken.None);

            Assert.IsNull(result);
        }

        [Test]
        public void ValidateAsync_WhenTokenHasExpiredButRefreshTokenIsStillActive_DoesNotThrow()
        {
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(SerenaWilliams.User);
            var jan4 = new DateTime(2000, 1, 4);
            _dateProviderMock.Setup(dp => dp.Now).Returns(jan4);

            Assert.DoesNotThrowAsync(async () => await _tokenService.ValidateAsync(TokenInfo, RefreshTokenInfo, 
                SerenaWilliams.Device, CancellationToken.None));
        }

        [Test]
        public void ValidateAsync_WhenTokenPointsToNotExistingUser_ThrowsTokenValidationException()
        {
            User? userNotFound = null;
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(userNotFound);

            var exception = Assert.ThrowsAsync<TokenValidationException>(async () 
                => await _tokenService.ValidateAsync(TokenInfo, RefreshTokenInfo, SerenaWilliams.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.TokenInvalidUserIdMessage));
        }

        [Test]
        public void ValidateAsync_WhenTokenPointsToDifferentUserThanRefreshToken_ThrowsTokenValidationException()
        {
            var refreshTokenInfoWithDifferentUserId = new RefreshTokenInfo(SerenaWilliams.RefreshToken, "fake-jti", 
                JohnCena.UserId, SerenaWilliams.DeviceId, new(new(2000, 1, 1), new(2000, 1, 5)));

            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(SerenaWilliams.User);

            var exception = Assert.ThrowsAsync<TokenValidationException>(async () 
                => await _tokenService.ValidateAsync(TokenInfo, refreshTokenInfoWithDifferentUserId, 
                SerenaWilliams.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.RefreshTokenWrongUserMessage));
        }

        [Test]
        [TestCase("2000-01-01", Description = "Token is still active from Jan 1st to Jan 3rd")]
        [TestCase("2000-01-02", Description = "Token is still active from Jan 1st to Jan 3rd")]
        [TestCase("2000-01-03", Description = "Token is still active from Jan 1st to Jan 3rd")]
        public void ValidateAsync_WhenTokenIsStillActive_ThrowsTokenValidationException(DateTime expiredDate)
        {
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())).ReturnsAsync(SerenaWilliams.User);
            _dateProviderMock.Setup(dp => dp.Now).Returns(expiredDate);

            var exception = Assert.ThrowsAsync<TokenValidationException>(async () 
                => await _tokenService.ValidateAsync(TokenInfo, RefreshTokenInfo, SerenaWilliams.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.TokenStillActiveMessage));
        }

        [Test]
        [TestCase("1999-12-31", Description = "Refresh token is not valid till Jan 1st")]
        [TestCase("2000-01-06", Description = "Refresh token is valid till Jan 5rd")]
        public void ValidateAsync_WhenRefreshTokenIsNoLongerActive_ThrowsTokenValidationException(DateTime expiredDate)
        {
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())).ReturnsAsync(SerenaWilliams.User);
            _dateProviderMock.Setup(dp => dp.Now).Returns(expiredDate);

            var exception = Assert.ThrowsAsync<TokenValidationException>(async () 
                => await _tokenService.ValidateAsync(TokenInfo, RefreshTokenInfo, SerenaWilliams.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.RefreshTokenNotActiveMessage));
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenHasBeenInvalidated_ThrowsTokenValidationException()
        {
            RefreshTokenInfo.Invalidated = true;
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())).ReturnsAsync(SerenaWilliams.User);

            var exception = Assert.ThrowsAsync<TokenValidationException>(async () 
                => await _tokenService.ValidateAsync(TokenInfo, RefreshTokenInfo, SerenaWilliams.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.RefreshTokenInvalidatedMessage));
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenHasBeenUsedAlready_ThrowsTokenValidationException()
        {
            RefreshTokenInfo.Used = true;
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())).ReturnsAsync(SerenaWilliams.User);

            var exception = Assert.ThrowsAsync<TokenValidationException>(async ()
                => await _tokenService.ValidateAsync(TokenInfo, RefreshTokenInfo, SerenaWilliams.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.RefreshTokenUsedMessage));
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenWasIssuedForDifferentToken_ThrowsTokenValidationException()
        {
            var johnCenasToken = new TokenInfo(JohnCena.Token, "john-cena-fake-jti", JohnCena.UserId,
                new(new(2000, 1, 1), new(2000, 1, 3)));
            var serenaWilliamsRefreshToken = RefreshTokenInfo;

            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())).ReturnsAsync(SerenaWilliams.User);

            var exception = Assert.ThrowsAsync<TokenValidationException>(async ()
                => await _tokenService.ValidateAsync(johnCenasToken, serenaWilliamsRefreshToken, SerenaWilliams.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.RefreshTokenWrongTokenMessage));
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenWasIssuedForDifferentDevice_ThrowsTokenValidationException()
        {
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())).ReturnsAsync(SerenaWilliams.User);

            var differentDevice = JohnCena.Device;
            var exception = Assert.ThrowsAsync<TokenValidationException>(async ()
                => await _tokenService.ValidateAsync(TokenInfo, RefreshTokenInfo, differentDevice, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.RefreshTokenWrongDeviceMessage));
        }
    }
}

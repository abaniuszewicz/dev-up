using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Common.Services;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.Services.Exceptions;
using DevUp.Domain.Identity.Setup;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.Services
{
    public class TokenServiceTests
    {
        private IdentityFaker _faker;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private Mock<IDeviceRepository> _deviceRepositoryMock;
        private Mock<IDateTimeProvider> _dateProviderMock;
        private IOptions<AuthenticationOptions> _authenticationOptions;

        private ITokenService _tokenService;

        [SetUp]
        public void SetUp()
        {
            _faker = new IdentityFaker();
            _userRepositoryMock = new Mock<IUserRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _deviceRepositoryMock = new Mock<IDeviceRepository>();
            _dateProviderMock = new Mock<IDateTimeProvider>();

            var expiry = _faker.Faker.Date.Timespan();
            _authenticationOptions = Options.Create(new AuthenticationOptions()
            {
                TokenExpiry = expiry,
                RefreshTokenExpiry = 5 * expiry,
                SigningKey = _faker.Faker.Random.String2(32)
            });

            _tokenService = new TokenService(_userRepositoryMock.Object, _refreshTokenRepositoryMock.Object, _deviceRepositoryMock.Object, _dateProviderMock.Object, _authenticationOptions);
        }

        [Test]
        public async Task CreateAsync_WhenCalled_StoresDeviceAndRefreshTokenInRepository()
        {
            _dateProviderMock.Setup(dp => dp.Now)
                .Returns(_faker.Faker.Date.Soon());

            await _tokenService.CreateAsync(_faker.User, _faker.Device, CancellationToken.None);

            _deviceRepositoryMock.Verify(dr => dr.AddAsync(_faker.Device, It.IsAny<CancellationToken>()), Times.Once);

            var expectedLifetime = new DateTimeRange(
                _dateProviderMock.Object.Now,
                _dateProviderMock.Object.Now.Add(_authenticationOptions.Value.RefreshTokenExpiry));
            _refreshTokenRepositoryMock.Verify(rtr => rtr.AddAsync(
                It.Is<RefreshTokenInfo>(rti => rti.Lifespan == expectedLifetime && rti.UserId == _faker.UserId && rti.DeviceId == _faker.DeviceId), 
                It.IsAny<CancellationToken>())
            , Times.Once);
        }

        [Test]
        public async Task DescribeAsync_WhenCalledWithRefreshToken_FetchesDescribedValueFromRepository()
        {
            await _tokenService.DescribeAsync(_faker.RefreshToken, CancellationToken.None);
            _refreshTokenRepositoryMock.Verify(rtr => rtr.GetByIdAsync(_faker.RefreshToken, CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task DescribeAsync_WhenCalledWithRefreshTokenThatExistsInRepository_ReturnsDescribedValue()
        {
            _refreshTokenRepositoryMock.Setup(rtr => rtr.GetByIdAsync(_faker.RefreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.RefreshTokenInfo);

            var result = await _tokenService.DescribeAsync(_faker.RefreshToken, CancellationToken.None);

            Assert.AreEqual(_faker.RefreshTokenInfo, result);
        }

        [Test]
        public async Task DescribeAsync_WhenCalledWithRefreshTokenThatDoesNotExistInRepository_ReturnsNull()
        {
            _refreshTokenRepositoryMock.Setup(rtr => rtr.GetByIdAsync(_faker.RefreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshTokenInfo?)null);

            var result = await _tokenService.DescribeAsync(_faker.RefreshToken, CancellationToken.None);

            Assert.IsNull(result);
        }

        [Test]
        public void ValidateAsync_WhenTokenHasExpiredButRefreshTokenIsStillActive_DoesNotThrow()
        {
            var now = _faker.Faker.Date.Between(_faker.TokenInfo.Lifespan.End, _faker.RefreshTokenInfo.Lifespan.End);
            _dateProviderMock.Setup(dp => dp.Now)
                .Returns(now);
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);

            Assert.DoesNotThrowAsync(async () 
                => await _tokenService.ValidateAsync(_faker.TokenInfo, _faker.RefreshTokenInfo, _faker.Device, CancellationToken.None));
        }

        [Test]
        public void ValidateAsync_WhenTokenPointsToNotExistingUser_ThrowsTokenValidationException()
        {
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var exception = Assert.ThrowsAsync<TokenValidationException>(async () 
                => await _tokenService.ValidateAsync(_faker.TokenInfo, _faker.RefreshTokenInfo, _faker.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.TokenInvalidUserIdMessage));
        }

        [Test]
        public void ValidateAsync_WhenTokenPointsToDifferentUserThanRefreshToken_ThrowsTokenValidationException()
        {
            var differentUserId = new IdentityFaker().UserId;
            var refreshTokenInfo = new RefreshTokenInfo(_faker.RefreshToken, _faker.RefreshTokenInfo.Jti, 
                differentUserId, _faker.DeviceId, _faker.RefreshTokenInfo.Lifespan);

            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);

            var exception = Assert.ThrowsAsync<TokenValidationException>(async () 
                => await _tokenService.ValidateAsync(_faker.TokenInfo, refreshTokenInfo, _faker.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.RefreshTokenWrongUserMessage));
        }

        [Test]
        public void ValidateAsync_WhenTokenIsStillActive_ThrowsTokenValidationException()
        {
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);

            var dates = new[]
            {
                _faker.TokenInfo.Lifespan.Start,
                _faker.Faker.Date.Between(_faker.TokenInfo.Lifespan.Start, _faker.TokenInfo.Lifespan.End),
                _faker.TokenInfo.Lifespan.End
            };

            foreach (var date in dates)
            {
                _dateProviderMock.Setup(dp => dp.Now).Returns(date);

                var exception = Assert.ThrowsAsync<TokenValidationException>(async ()
                    => await _tokenService.ValidateAsync(_faker.TokenInfo, _faker.RefreshTokenInfo, _faker.Device, CancellationToken.None));

                Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.TokenStillActiveMessage));
            }
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenIsNoLongerActive_ThrowsTokenValidationException()
        {
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);

            var dates = new[]
            {
                _faker.Faker.Date.Recent(1, _faker.RefreshTokenInfo.Lifespan.Start),
                _faker.Faker.Date.Soon(1, _faker.RefreshTokenInfo.Lifespan.End)
            };

            foreach (var date in dates)
            {
                _dateProviderMock.Setup(dp => dp.Now).Returns(date);

                var exception = Assert.ThrowsAsync<TokenValidationException>(async ()
                    => await _tokenService.ValidateAsync(_faker.TokenInfo, _faker.RefreshTokenInfo, _faker.Device, CancellationToken.None));

                Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.RefreshTokenNotActiveMessage));
            }
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenHasBeenInvalidated_ThrowsTokenValidationException()
        {
            var rti = _faker.RefreshTokenInfo;
            rti.Invalidated = true;
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);

            var exception = Assert.ThrowsAsync<TokenValidationException>(async () 
                => await _tokenService.ValidateAsync(_faker.TokenInfo, rti, _faker.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.RefreshTokenInvalidatedMessage));
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenHasBeenUsedAlready_ThrowsTokenValidationException()
        {
            var rti = _faker.RefreshTokenInfo;
            rti.Used = true;
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);

            var exception = Assert.ThrowsAsync<TokenValidationException>(async ()
                => await _tokenService.ValidateAsync(_faker.TokenInfo, rti, _faker.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.RefreshTokenUsedMessage));
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenWasIssuedForDifferentToken_ThrowsTokenValidationException()
        {
            var differentRefreshTokenInfo = new IdentityFaker().RefreshTokenInfo;
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);

            var exception = Assert.ThrowsAsync<TokenValidationException>(async ()
                => await _tokenService.ValidateAsync(_faker.TokenInfo, differentRefreshTokenInfo, _faker.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.RefreshTokenWrongTokenMessage));
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenWasIssuedForDifferentDevice_ThrowsTokenValidationException()
        {
            var differentDevice = new IdentityFaker().Device;
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);

            var exception = Assert.ThrowsAsync<TokenValidationException>(async ()
                => await _tokenService.ValidateAsync(_faker.TokenInfo, _faker.RefreshTokenInfo, differentDevice, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(TokenValidationException.RefreshTokenWrongDeviceMessage));
        }
    }
}

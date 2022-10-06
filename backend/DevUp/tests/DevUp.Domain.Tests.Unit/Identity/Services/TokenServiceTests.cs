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
using Microsoft.IdentityModel.Tokens;
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
                SigningKey = _faker.Faker.Random.String2(32),
                Algorithm = _faker.Faker.PickRandom(SecurityAlgorithms.HmacSha256, SecurityAlgorithms.HmacSha512)
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
            _refreshTokenRepositoryMock.Setup(rtr => rtr.GetByIdAsync(_faker.RefreshTokenInfoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.RefreshTokenInfo);

            await _tokenService.DescribeAsync(_faker.RefreshToken, CancellationToken.None);

            _refreshTokenRepositoryMock.Verify(rtr => rtr.GetByIdAsync(_faker.RefreshTokenInfoId, CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task DescribeAsync_WhenCalledWithRefreshTokenThatExistsInRepository_ReturnsDescribedValue()
        {
            _refreshTokenRepositoryMock.Setup(rtr => rtr.GetByIdAsync(_faker.RefreshTokenInfoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.RefreshTokenInfo);

            var result = await _tokenService.DescribeAsync(_faker.RefreshToken, CancellationToken.None);

            Assert.AreEqual(_faker.RefreshTokenInfo, result);
        }

        [Test]
        public void DescribeAsync_WhenCalledWithRefreshTokenThatDoesNotExistInRepository_ThrowsRefreshTokenInfoNotFoundException()
        {
            _refreshTokenRepositoryMock.Setup(rtr => rtr.GetByIdAsync(_faker.RefreshTokenInfoId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((RefreshTokenInfo?)null);

            Assert.ThrowsAsync<RefreshTokenInfoIdNotFoundException>(async ()
                => await _tokenService.DescribeAsync(_faker.RefreshToken, CancellationToken.None));
        }

        [Test]
        public void ValidateAsync_WhenTokenHasExpiredButRefreshTokenIsStillActive_DoesNotThrow()
        {
            var now = _faker.Faker.Date.Between(_faker.TokenInfo.Lifespan.End, _faker.RefreshTokenInfo.Lifespan.End);
            _dateProviderMock.Setup(dp => dp.Now)
                .Returns(now);
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);
            _deviceRepositoryMock.Setup(dr => dr.GetByIdAsync(_faker.DeviceId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.Device);

            Assert.DoesNotThrowAsync(async () 
                => await _tokenService.ValidateAsync(_faker.TokenInfo, _faker.RefreshTokenInfo, _faker.Device, CancellationToken.None));
        }

        [Test]
        public void ValidateAsync_WhenTokenPointsToNotExistingUser_ThrowsUserIdNotFoundException()
        {
            var now = _faker.Faker.Date.Between(_faker.TokenInfo.Lifespan.End, _faker.RefreshTokenInfo.Lifespan.End);
            _dateProviderMock.Setup(dp => dp.Now)
                .Returns(now);
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var exception = Assert.ThrowsAsync<UserIdNotFoundException>(async () 
                => await _tokenService.ValidateAsync(_faker.TokenInfo, _faker.RefreshTokenInfo, _faker.Device, CancellationToken.None));
        }

        [Test]
        public void ValidateAsync_WhenTokenPointsToDifferentUserThanRefreshToken_ThrowsUserIdMismatchException()
        {
            var now = _faker.Faker.Date.Between(_faker.TokenInfo.Lifespan.End, _faker.RefreshTokenInfo.Lifespan.End);
            _dateProviderMock.Setup(dp => dp.Now)
                .Returns(now);
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);

            var differentUserId = new IdentityFaker().UserId;
            var refreshTokenInfo = new RefreshTokenInfo(_faker.RefreshTokenInfoId, _faker.RefreshTokenInfo.Jti,
                differentUserId, _faker.DeviceId, _faker.RefreshTokenInfo.Lifespan);

            var exception = Assert.ThrowsAsync<UserIdMismatchException>(async () 
                => await _tokenService.ValidateAsync(_faker.TokenInfo, refreshTokenInfo, _faker.Device, CancellationToken.None));
        }

        [Test]
        public void ValidateAsync_WhenTokenIsStillActive_ThrowsTokenStillActiveException()
        {
            var dates = new[]
            {
                _faker.TokenInfo.Lifespan.Start,
                _faker.Faker.Date.Between(_faker.TokenInfo.Lifespan.Start, _faker.TokenInfo.Lifespan.End),
                _faker.TokenInfo.Lifespan.End
            };

            foreach (var date in dates)
            {
                _dateProviderMock.Setup(dp => dp.Now).Returns(date);

                var exception = Assert.ThrowsAsync<TokenStillActiveException>(async ()
                    => await _tokenService.ValidateAsync(_faker.TokenInfo, _faker.RefreshTokenInfo, _faker.Device, CancellationToken.None));
            }
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenIsNoLongerActive_ThrowsRefreshTokenNotActiveException()
        {
            var dates = new[]
            {
                _faker.Faker.Date.Recent(1, _faker.RefreshTokenInfo.Lifespan.Start),
                _faker.Faker.Date.Soon(1, _faker.RefreshTokenInfo.Lifespan.End)
            };

            foreach (var date in dates)
            {
                _dateProviderMock.Setup(dp => dp.Now).Returns(date);

                var exception = Assert.ThrowsAsync<RefreshTokenNotActiveException>(async ()
                    => await _tokenService.ValidateAsync(_faker.TokenInfo, _faker.RefreshTokenInfo, _faker.Device, CancellationToken.None));
            }
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenHasBeenInvalidated_ThrowsRefreshTokenInvalidatedException()
        {
            var rti = _faker.RefreshTokenInfo;
            rti.Invalidated = true;

            var exception = Assert.ThrowsAsync<RefreshTokenInvalidatedException>(async () 
                => await _tokenService.ValidateAsync(_faker.TokenInfo, rti, _faker.Device, CancellationToken.None));
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenHasBeenUsedAlready_ThrowsRefreshTokenUsedException()
        {
            var rti = _faker.RefreshTokenInfo;
            rti.Used = true;

            var exception = Assert.ThrowsAsync<RefreshTokenUsedException>(async ()
                => await _tokenService.ValidateAsync(_faker.TokenInfo, rti, _faker.Device, CancellationToken.None));
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenWasIssuedForDifferentToken_ThrowsTokenMismatchException()
        {
            var differentRefreshTokenInfo = new IdentityFaker().RefreshTokenInfo;

            var exception = Assert.ThrowsAsync<TokenMismatchException>(async ()
                => await _tokenService.ValidateAsync(_faker.TokenInfo, differentRefreshTokenInfo, _faker.Device, CancellationToken.None));
        }

        [Test]
        public void ValidateAsync_WhenRefreshTokenWasIssuedForDifferentDevice_ThrowsDeviceIdMismatchException()
        {
            var differentDeviceId = new IdentityFaker().DeviceId;
            var rti = new RefreshTokenInfo(_faker.RefreshTokenInfoId, _faker.TokenInfo.Jti, _faker.UserId, differentDeviceId, _faker.RefreshTokenInfo.Lifespan);
            var now = _faker.Faker.Date.Between(_faker.TokenInfo.Lifespan.End, _faker.RefreshTokenInfo.Lifespan.End);
            _dateProviderMock.Setup(dp => dp.Now)
                .Returns(now);
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);
            _deviceRepositoryMock.Setup(dr => dr.GetByIdAsync(_faker.DeviceId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.Device);

            var exception = Assert.ThrowsAsync<DeviceIdMismatchException>(async ()
                => await _tokenService.ValidateAsync(_faker.TokenInfo, rti, _faker.Device, CancellationToken.None));
        }

        [Test]
        public void ValidateAsync_WhenRequestedRefreshFromDifferentDevice_ThrowsDeviceIdMismatchException()
        {
            var differentDevice = new IdentityFaker().Device;
            var now = _faker.Faker.Date.Between(_faker.TokenInfo.Lifespan.End, _faker.RefreshTokenInfo.Lifespan.End);
            _dateProviderMock.Setup(dp => dp.Now)
                .Returns(now);
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);
            _deviceRepositoryMock.Setup(dr => dr.GetByIdAsync(_faker.DeviceId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.Device);

            var exception = Assert.ThrowsAsync<DeviceIdMismatchException>(async ()
                => await _tokenService.ValidateAsync(_faker.TokenInfo, _faker.RefreshTokenInfo, differentDevice, CancellationToken.None));
        }
    }
}

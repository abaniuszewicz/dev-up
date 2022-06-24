using System.Threading;
using System.Threading.Tasks;
using DevUp.Common;
using DevUp.Domain.Identity;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.Services;
using Moq;
using NUnit.Framework;

using static DevUp.Tests.Utilities.ObjectMothers.Identity.IdentityObjectMother;

namespace DevUp.Domain.Tests.Unit.Identity.Services
{
    public class TokenServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private Mock<IDeviceRepository> _deviceRepositoryMock;
        private Mock<IDateTimeProvider> _dateProviderMock;
        private JwtSettings _jwtSettings;

        private ITokenService _tokenService;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _deviceRepositoryMock = new Mock<IDeviceRepository>();
            _dateProviderMock = new Mock<IDateTimeProvider>();
            _jwtSettings = null; // mock?

            _tokenService = new TokenService(_userRepositoryMock.Object, _refreshTokenRepositoryMock.Object, _deviceRepositoryMock.Object, _dateProviderMock.Object, _jwtSettings);
        }

        [Test]
        public async Task DescribeAsync_WhenCalledWithRefreshToken_FetchesInfoFromRepository()
        {
            await _tokenService.DescribeAsync(JohnCena.RefreshToken, CancellationToken.None);
            _refreshTokenRepositoryMock.Verify(rtr => rtr.GetByIdAsync(JohnCena.RefreshToken, CancellationToken.None), Times.Once);
        }
    }
}

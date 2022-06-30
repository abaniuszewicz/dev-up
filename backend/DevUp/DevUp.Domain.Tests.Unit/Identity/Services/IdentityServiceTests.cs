using System;
using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Common.Types;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Enums;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.ValueObjects;
using Moq;
using NUnit.Framework;

using static DevUp.Tests.Utilities.ObjectMothers.Identity.IdentityObjectMother;

namespace DevUp.Domain.Tests.Unit.Identity.Services
{
    public class IdentityServiceTests
    {
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private Mock<IPasswordService> _passwordServiceMock;
        private Mock<ITokenService> _tokenServiceMock;

        private IIdentityService _identityService;

        [SetUp]
        public void SetUp()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _passwordServiceMock = new Mock<IPasswordService>();
            _tokenServiceMock = new Mock<ITokenService>();

            _identityService = new IdentityService(_userRepositoryMock.Object, _refreshTokenRepositoryMock.Object, _passwordServiceMock.Object, _tokenServiceMock.Object);
        }

        [Test]
        public void RegisterAsync_WhenUsernameTaken_ThrowsRegisterException()
        {
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(JohnCena.User);

            var exception = Assert.ThrowsAsync<RegisterException>(async () => await _identityService.RegisterAsync(JohnCena.Username, Unused.Password, Unused.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(RegisterException.UsernameTakenMessage));
        }

        [Test]
        public void RegisterAsync_WhenFailedToStoreNewlyCreatedUser_ThrowsRegisterException()
        {
            User? usernameNotTaken = null;
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(usernameNotTaken);

            _passwordServiceMock.Setup(ps => ps.HashAsync(It.IsAny<Password>(), It.IsAny<CancellationToken>())).ReturnsAsync(Unused.PasswordHash);

            User? userWasNotPersisted = null;
            _userRepositoryMock.Setup(ur => ur.CreateAsync(It.IsAny<Username>(), It.IsAny<PasswordHash>(), It.IsAny<CancellationToken>())).ReturnsAsync(userWasNotPersisted);

            var exception = Assert.ThrowsAsync<RegisterException>(async () => await _identityService.RegisterAsync(Unused.Username, Unused.Password, Unused.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(RegisterException.CreationFailedMessage));
        }

        [Test]
        public async Task RegisterAsync_WhenRegistrationSucceeded_ReturnsTokenPair()
        {
            User? usernameNotTaken = null;
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(usernameNotTaken);

            _userRepositoryMock.Setup(ur => ur.CreateAsync(It.IsAny<Username>(), It.IsAny<PasswordHash>(), It.IsAny<CancellationToken>())).ReturnsAsync(JohnCena.User);

            _tokenServiceMock.Setup(ts => ts.CreateAsync(It.IsAny<User>(), It.IsAny<Device>(), It.IsAny<CancellationToken>())).ReturnsAsync((JohnCena.Token, JohnCena.RefreshToken));

            var result = await _identityService.RegisterAsync(JohnCena.Username, JohnCena.Password, JohnCena.Device, CancellationToken.None);
            Assert.NotNull(result);
            Assert.AreEqual(result.Token, JohnCena.Token);
            Assert.AreEqual(result.RefreshToken, JohnCena.RefreshToken);
        }

        [Test]
        public void LoginAsync_WhenGivenNotRegisteredUsername_ThrowsLoginException()
        {
            User? usernameNotFound = null;
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(usernameNotFound);

            var exception = Assert.ThrowsAsync<LoginException>(async () => await _identityService.LoginAsync(Unused.Username, Unused.Password, Unused.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(LoginException.InvalidUsernameMessage));
        }

        [Test]
        public void LoginAsync_WhenFailedToRetrievePasswordHash_ThrowsLoginException()
        {
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(JohnCena.User);

            PasswordHash? passwordHashNotFound = null;
            _userRepositoryMock.Setup(ur => ur.GetPasswordHashAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(passwordHashNotFound);

            var exception = Assert.ThrowsAsync<LoginException>(async () => await _identityService.LoginAsync(JohnCena.Username, JohnCena.Password, JohnCena.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(LoginException.HashNotFoundMessage));
        }

        [Test]
        public void LoginAsync_WhenGivenInvalidPassword_ThrowsLoginException()
        {
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(JohnCena.User);

            var differentPasswordHash = Unused.PasswordHash;
            _userRepositoryMock.Setup(ur => ur.GetPasswordHashAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(differentPasswordHash);

            _passwordServiceMock.Setup(ps => ps.VerifyAsync(It.IsAny<Password>(), It.IsAny<PasswordHash>(), It.IsAny<CancellationToken>())).ReturnsAsync(PasswordVerifyResult.Failed);

            var differentPassword = Unused.Password;
            var exception = Assert.ThrowsAsync<LoginException>(async () => await _identityService.LoginAsync(JohnCena.Username, differentPassword, JohnCena.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(LoginException.InvalidPasswordMessage));
        }

        [Test]
        public async Task LoginAsync_WhenGivenValidCredentials_ReturnsTokenPair()
        {
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(JohnCena.User);
            _userRepositoryMock.Setup(ur => ur.GetPasswordHashAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(JohnCena.PasswordHash);
            _passwordServiceMock.Setup(ps => ps.VerifyAsync(It.IsAny<Password>(), It.IsAny<PasswordHash>(), It.IsAny<CancellationToken>())).ReturnsAsync(PasswordVerifyResult.Success);

            _tokenServiceMock.Setup(ts => ts.CreateAsync(It.IsAny<User>(), It.IsAny<Device>(), It.IsAny<CancellationToken>())).ReturnsAsync((JohnCena.Token, JohnCena.RefreshToken));

            var result = await _identityService.LoginAsync(JohnCena.Username, JohnCena.Password, JohnCena.Device, CancellationToken.None);
            Assert.NotNull(result);
            Assert.AreEqual(result.Token, JohnCena.Token);
            Assert.AreEqual(result.RefreshToken, JohnCena.RefreshToken);
        }

        [Test]
        public void RefreshAsync_WhenGivenInvalidToken_ThrowsRefreshException()
        {
            TokenInfo? invalidToken = null;
            _tokenServiceMock.Setup(ts => ts.DescribeAsync(It.IsAny<Token>(), It.IsAny<CancellationToken>())).ReturnsAsync(invalidToken);

            var exception = Assert.ThrowsAsync<RefreshException>(async () => await _identityService.RefreshAsync(Unused.Token, Unused.RefreshToken, Unused.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(RefreshException.InvalidTokenMessage));
        }

        [Test]
        public void RefreshAsync_WhenGivenInvalidRefreshToken_ThrowsRefreshException()
        {
            var lifespanDoesNotMatterHere = new DateTimeRange(new DateTime(2022, 6, 24, 0, 0, 0), new DateTime(2022, 6, 24, 15, 0, 0));

            var validToken = JohnCena.Token;
            var validTokenInfo = new TokenInfo(validToken, "fake-jti", JohnCena.UserId, lifespanDoesNotMatterHere);
            _tokenServiceMock.Setup(ts => ts.DescribeAsync(It.IsAny<Token>(), It.IsAny<CancellationToken>())).ReturnsAsync(validTokenInfo);

            RefreshTokenInfo? invalidRefreshTokenInfo = null;
            _tokenServiceMock.Setup(ts => ts.DescribeAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>())).ReturnsAsync(invalidRefreshTokenInfo);

            var exception = Assert.ThrowsAsync<RefreshException>(async () => await _identityService.RefreshAsync(JohnCena.Token, SerenaWilliams.RefreshToken, JohnCena.Device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(RefreshException.InvalidRefreshTokenMessage));
        }

        [Test]
        public async Task RefreshAsync_WhenGivenValidTokenPair_MarksRefreshTokenAsUsed()
        {
            var lifespanDoesNotMatterHere = new DateTimeRange(new DateTime(2022, 6, 24, 0, 0, 0), new DateTime(2022, 6, 24, 15, 0, 0));

            var token = JohnCena.Token;
            var tokenInfo = new TokenInfo(token, "fake-jti", JohnCena.UserId, lifespanDoesNotMatterHere);
            _tokenServiceMock.Setup(ts => ts.DescribeAsync(It.IsAny<Token>(), It.IsAny<CancellationToken>())).ReturnsAsync(tokenInfo);

            var refreshToken = JohnCena.RefreshToken;
            var refreshTokenInfo = new RefreshTokenInfo(refreshToken, "fake-jti", JohnCena.UserId, JohnCena.DeviceId, lifespanDoesNotMatterHere);
            _tokenServiceMock.Setup(ts => ts.DescribeAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>())).ReturnsAsync(refreshTokenInfo);

            await _identityService.RefreshAsync(token, refreshToken, JohnCena.Device, CancellationToken.None);

            _refreshTokenRepositoryMock.Verify(rtr => rtr.MarkAsUsedAsync(refreshTokenInfo, CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task RefreshAsync_WhenGivenValidTokenPair_ReturnsNewTokenPair()
        {
            var lifespanDoesNotMatterHere = new DateTimeRange(new DateTime(2022, 6, 24, 0, 0, 0), new DateTime(2022, 6, 24, 15, 0, 0));

            var token = JohnCena.Token;
            var tokenInfo = new TokenInfo(token, "fake-jti", JohnCena.UserId, lifespanDoesNotMatterHere);
            _tokenServiceMock.Setup(ts => ts.DescribeAsync(It.IsAny<Token>(), It.IsAny<CancellationToken>())).ReturnsAsync(tokenInfo);

            var refreshToken = JohnCena.RefreshToken;
            var refreshTokenInfo = new RefreshTokenInfo(refreshToken, "fake-jti", JohnCena.UserId, JohnCena.DeviceId, lifespanDoesNotMatterHere);
            _tokenServiceMock.Setup(ts => ts.DescribeAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>())).ReturnsAsync(refreshTokenInfo);

            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(It.IsAny<UserId>(), It.IsAny<CancellationToken>())).ReturnsAsync(JohnCena.User);

            var newToken = Unused.Token;
            var newRefreshToken = Unused.RefreshToken;
            _tokenServiceMock.Setup(ts => ts.CreateAsync(It.IsAny<User>(), It.IsAny<Device>(), It.IsAny<CancellationToken>())).ReturnsAsync((newToken, newRefreshToken));

            var result = await _identityService.RefreshAsync(token, refreshToken, JohnCena.Device, CancellationToken.None);

            _tokenServiceMock.Verify(ts => ts.CreateAsync(JohnCena.User, JohnCena.Device, CancellationToken.None), Times.Once);
            Assert.AreEqual(newToken, result.Token);
            Assert.AreEqual(newRefreshToken, result.RefreshToken);
        }
    }
}

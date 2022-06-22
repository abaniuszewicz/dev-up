using System;
using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Enums;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.ValueObjects;
using DevUp.Tests.Utilities.ObjectMothers.Identity;
using Moq;
using NUnit.Framework;

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
        public void RegisterAsync_WhenUsernameTaken_ThrowsRegisterExceptionWithErrorDescription()
        {
            var johnCena = IdentityObjectMother.JohnCena;
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(johnCena.User);

            var password = IdentityObjectMother.Unused.Password;
            var device = IdentityObjectMother.Unused.Device;
            var exception = Assert.ThrowsAsync<RegisterException>(async () => await _identityService.RegisterAsync(johnCena.Username, password, device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(RegisterException.UsernameTakenMessage));
        }

        [Test]
        public void RegisterAsync_WhenFailedToStoreNewlyCreatedUser_ThrowsRegisterExceptionWithErrorDescription()
        {
            User? usernameNotTaken = null;
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(usernameNotTaken);

            var passwordHash = IdentityObjectMother.Unused.PasswordHash;
            _passwordServiceMock.Setup(ps => ps.HashAsync(It.IsAny<Password>(), It.IsAny<CancellationToken>())).ReturnsAsync(passwordHash);

            User? userWasNotPersisted = null;
            _userRepositoryMock.Setup(ur => ur.CreateAsync(It.IsAny<Username>(), It.IsAny<PasswordHash>(), It.IsAny<CancellationToken>())).ReturnsAsync(userWasNotPersisted);

            var username = IdentityObjectMother.Unused.Username;
            var password = IdentityObjectMother.Unused.Password;
            var device = IdentityObjectMother.Unused.Device;
            var exception = Assert.ThrowsAsync<RegisterException>(async () => await _identityService.RegisterAsync(username, password, device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(RegisterException.CreationFailedMessage));
        }

        [Test]
        public async Task RegisterAsync_WhenRegistrationSucceeded_ReturnsTokenPair()
        {
            User? usernameNotTaken = null;
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(usernameNotTaken);

            var johnCena = IdentityObjectMother.JohnCena;
            _userRepositoryMock.Setup(ur => ur.CreateAsync(It.IsAny<Username>(), It.IsAny<PasswordHash>(), It.IsAny<CancellationToken>())).ReturnsAsync(johnCena.User);

            var token = IdentityObjectMother.Unused.Token;
            var refreshToken = IdentityObjectMother.Unused.RefreshToken;
            _tokenServiceMock.Setup(ts => ts.CreateAsync(It.IsAny<User>(), It.IsAny<Device>(), It.IsAny<CancellationToken>())).ReturnsAsync((token, refreshToken));

            var result = await _identityService.RegisterAsync(johnCena.Username, johnCena.Password, johnCena.Device, CancellationToken.None);
            Assert.NotNull(result);
            Assert.AreEqual(result.Token, token);
            Assert.AreEqual(result.RefreshToken, refreshToken);
        }

        [Test]
        public void LoginAsync_WhenGivenNotRegisteredUsername_ThrowsLoginExceptionWithErrorDescription()
        {
            User? usernameNotFound = null;
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(usernameNotFound);

            var username = IdentityObjectMother.Unused.Username;
            var password = IdentityObjectMother.Unused.Password;
            var device = IdentityObjectMother.Unused.Device;
            var exception = Assert.ThrowsAsync<LoginException>(async () => await _identityService.LoginAsync(username, password, device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(LoginException.InvalidUsernameMessage));
        }

        [Test]
        public void LoginAsync_WhenFailedToRetrievePasswordHash_ThrowsLoginExceptionWithErrorDescription()
        {
            var johnCena = IdentityObjectMother.JohnCena;
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(johnCena.User);

            PasswordHash? passwordHashNotFound = null;
            _userRepositoryMock.Setup(ur => ur.GetPasswordHashAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(passwordHashNotFound);

            var username = johnCena.Username;
            var password = johnCena.Password;
            var device = johnCena.Device;
            var exception = Assert.ThrowsAsync<LoginException>(async () => await _identityService.LoginAsync(username, password, device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(LoginException.HashNotFoundMessage));
        }

        [Test]
        public void LoginAsync_WhenGivenInvalidPassword_ThrowsLoginExceptionWithErrorDescription()
        {
            var johnCena = IdentityObjectMother.JohnCena;
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(johnCena.User);

            var differentPasswordHash = IdentityObjectMother.Unused.PasswordHash;
            _userRepositoryMock.Setup(ur => ur.GetPasswordHashAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(differentPasswordHash);

            _passwordServiceMock.Setup(ps => ps.VerifyAsync(It.IsAny<Password>(), It.IsAny<PasswordHash>(), It.IsAny<CancellationToken>())).ReturnsAsync(PasswordVerifyResult.Failed);

            var username = johnCena.Username;
            var differentPassword = IdentityObjectMother.Unused.Password;
            var device = johnCena.Device;
            var exception = Assert.ThrowsAsync<LoginException>(async () => await _identityService.LoginAsync(username, differentPassword, device, CancellationToken.None));
            Assert.That(exception!.Errors, Has.One.EqualTo(LoginException.InvalidPasswordMessage));
        }

        [Test]
        public async Task LoginAsync_WhenGivenValidCredentials_ReturnsTokenPair()
        {
            var johnCena = IdentityObjectMother.JohnCena;
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(It.IsAny<Username>(), It.IsAny<CancellationToken>())).ReturnsAsync(johnCena.User);
            _userRepositoryMock.Setup(ur => ur.GetPasswordHashAsync(It.IsAny<User>(), It.IsAny<CancellationToken>())).ReturnsAsync(johnCena.PasswordHash);
            _passwordServiceMock.Setup(ps => ps.VerifyAsync(It.IsAny<Password>(), It.IsAny<PasswordHash>(), It.IsAny<CancellationToken>())).ReturnsAsync(PasswordVerifyResult.Success);

            var token = johnCena.Token;
            var refreshToken = johnCena.RefreshToken;
            _tokenServiceMock.Setup(ts => ts.CreateAsync(It.IsAny<User>(), It.IsAny<Device>(), It.IsAny<CancellationToken>())).ReturnsAsync((token, refreshToken));

            var result = await _identityService.LoginAsync(johnCena.Username, johnCena.Password, johnCena.Device, CancellationToken.None);
            Assert.NotNull(result);
            Assert.AreEqual(result.Token, token);
            Assert.AreEqual(result.RefreshToken, refreshToken);
        }
    }
}

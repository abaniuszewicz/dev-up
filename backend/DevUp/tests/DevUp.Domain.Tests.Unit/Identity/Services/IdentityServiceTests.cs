﻿using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Repositories;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.Services.Enums;
using DevUp.Domain.Identity.Services.Exceptions;
using DevUp.Domain.Identity.ValueObjects;
using Moq;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.Services
{
    public class IdentityServiceTests
    {
        private IdentityFaker _faker;
        private Mock<IUserRepository> _userRepositoryMock;
        private Mock<IRefreshTokenRepository> _refreshTokenRepositoryMock;
        private Mock<IPasswordService> _passwordServiceMock;
        private Mock<ITokenService> _tokenServiceMock;

        private IIdentityService _identityService;

        [SetUp]
        public void SetUp()
        {
            _faker = new IdentityFaker();
            _userRepositoryMock = new Mock<IUserRepository>();
            _refreshTokenRepositoryMock = new Mock<IRefreshTokenRepository>();
            _passwordServiceMock = new Mock<IPasswordService>();
            _tokenServiceMock = new Mock<ITokenService>();

            _identityService = new IdentityService(_userRepositoryMock.Object, _refreshTokenRepositoryMock.Object, _passwordServiceMock.Object, _tokenServiceMock.Object);
        }

        [Test]
        public void RegisterAsync_WhenUsernameTaken_ThrowsUsernameTakenException()
        {
            var user = _faker.User;
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(_faker.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            var exception = Assert.ThrowsAsync<UsernameTakenException>(async () 
                => await _identityService.RegisterAsync(_faker.Username, _faker.Password, _faker.Device, CancellationToken.None));
        }

        [Test]
        public async Task RegisterAsync_WhenRegistrationSucceeded_ReturnsTokenPair()
        {
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(_faker.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);
            _passwordServiceMock.Setup(ps => ps.HashAsync(_faker.Password, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.PasswordHash);
            _userRepositoryMock.Setup(ur => ur.CreateAsync(_faker.Username, _faker.PasswordHash, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);
            _tokenServiceMock.Setup(ts => ts.CreateAsync(_faker.User, _faker.Device, It.IsAny<CancellationToken>()))
                .ReturnsAsync((_faker.Token, _faker.RefreshToken));

            var result = await _identityService.RegisterAsync(_faker.Username, _faker.Password, _faker.Device, CancellationToken.None);
            Assert.NotNull(result);
            Assert.AreEqual(result.Token, _faker.Token);
            Assert.AreEqual(result.RefreshToken, _faker.RefreshToken);
        }

        [Test]
        public void LoginAsync_WhenGivenNotRegisteredUsername_ThrowsUsernameNotFoundException()
        {
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(_faker.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            var exception = Assert.ThrowsAsync<UsernameNotFoundException>(async () 
                => await _identityService.LoginAsync(_faker.Username, _faker.Password, _faker.Device, CancellationToken.None));
        }

        [Test]
        public void LoginAsync_WhenFailedToRetrievePasswordHash_ThrowsPasswordHashNotFoundException()
        {
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(_faker.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);
            _userRepositoryMock.Setup(ur => ur.GetPasswordHashAsync(_faker.User, It.IsAny<CancellationToken>()))
                .ReturnsAsync((PasswordHash?)null);

            var exception = Assert.ThrowsAsync<PasswordHashNotFoundException>(async () 
                => await _identityService.LoginAsync(_faker.Username, _faker.Password, _faker.Device, CancellationToken.None));
        }

        [Test]
        public void LoginAsync_WhenGivenInvalidPassword_ThrowsInvalidPasswordException()
        {
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(_faker.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);
            _userRepositoryMock.Setup(ur => ur.GetPasswordHashAsync(_faker.User, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.PasswordHash);
            var differentPassword = new IdentityFaker().Password;
            _passwordServiceMock.Setup(ps => ps.VerifyAsync(differentPassword, _faker.PasswordHash, It.IsAny<CancellationToken>()))
                .ReturnsAsync(PasswordVerifyResult.Failed);

            var exception = Assert.ThrowsAsync<InvalidPasswordException>(async () 
                => await _identityService.LoginAsync(_faker.Username, differentPassword, _faker.Device, CancellationToken.None));
        }

        [Test]
        public async Task LoginAsync_WhenGivenValidCredentials_ReturnsTokenPair()
        {
            _userRepositoryMock.Setup(ur => ur.GetByUsernameAsync(_faker.Username, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);
            _userRepositoryMock.Setup(ur => ur.GetPasswordHashAsync(_faker.User, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.PasswordHash);
            _passwordServiceMock.Setup(ps => ps.VerifyAsync(_faker.Password, _faker.PasswordHash, It.IsAny<CancellationToken>()))
                .ReturnsAsync(PasswordVerifyResult.Success);
            _tokenServiceMock.Setup(ts => ts.CreateAsync(_faker.User, _faker.Device, It.IsAny<CancellationToken>()))
                .ReturnsAsync((_faker.Token, _faker.RefreshToken));

            var result = await _identityService.LoginAsync(_faker.Username, _faker.Password, _faker.Device, CancellationToken.None);
            Assert.NotNull(result);
            Assert.AreEqual(result.Token, _faker.Token);
            Assert.AreEqual(result.RefreshToken, _faker.RefreshToken);
        }

        [Test]
        public async Task RefreshAsync_WhenGivenValidTokenPair_MarksRefreshTokenAsUsed()
        {
            _tokenServiceMock.Setup(ts => ts.DescribeAsync(_faker.Token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.TokenInfo);
            _tokenServiceMock.Setup(ts => ts.DescribeAsync(_faker.RefreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.RefreshTokenInfo);

            await _identityService.RefreshAsync(_faker.Token, _faker.RefreshToken, _faker.Device, CancellationToken.None);

            _refreshTokenRepositoryMock.Verify(rtr => rtr.MarkAsUsedAsync(_faker.RefreshTokenInfo, CancellationToken.None), Times.Once);
        }

        [Test]
        public async Task RefreshAsync_WhenGivenValidTokenPair_ReturnsNewTokenPair()
        {
            _tokenServiceMock.Setup(ts => ts.DescribeAsync(_faker.Token, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.TokenInfo);
            _tokenServiceMock.Setup(ts => ts.DescribeAsync(_faker.RefreshToken, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.RefreshTokenInfo);
            _userRepositoryMock.Setup(ur => ur.GetByIdAsync(_faker.UserId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(_faker.User);

            var newFaker = new IdentityFaker();
            var newToken = newFaker.Token;
            var newRefreshToken = newFaker.RefreshToken;
            _tokenServiceMock.Setup(ts => ts.CreateAsync(_faker.User, _faker.Device, It.IsAny<CancellationToken>()))
                .ReturnsAsync((newToken, newRefreshToken));

            var result = await _identityService.RefreshAsync(_faker.Token, _faker.RefreshToken, _faker.Device, CancellationToken.None);

            _tokenServiceMock.Verify(ts => ts.CreateAsync(_faker.User, _faker.Device, CancellationToken.None), Times.Once);
            Assert.AreEqual(newToken, result.Token);
            Assert.AreEqual(newRefreshToken, result.RefreshToken);
        }
    }
}

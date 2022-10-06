using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Services;
using DevUp.Domain.Identity.Services.Enums;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.Services
{
    public class PasswordServiceTests
    {
        private IdentityFaker _faker;
        private Mock<IPasswordHasher<User>> _hasherMock;
        private PasswordService _passwordService;
        User? _user;

        [SetUp]
        public void SetUp()
        {
            _faker = new IdentityFaker();
            _hasherMock = new Mock<IPasswordHasher<User>>();
            _passwordService = new PasswordService(_hasherMock.Object);
            _user = null;
        }

        [Test]
        public async Task HashAsync_WhenCalled_ForwardsRequestToHasherAndReturnsItsResult()
        {
            _hasherMock.Setup(h => h.HashPassword(_user, _faker.Password.Value))
                .Returns(_faker.PasswordHash.Value);

            var hash = await _passwordService.HashAsync(_faker.Password, CancellationToken.None);

            Assert.AreEqual(_faker.PasswordHash, hash);
            _hasherMock.Verify(h => h.HashPassword(_user, _faker.Password.Value));
        }

        [Test]
        public async Task VerifyAsync_ForFailedVerification_ReturnsFailedResult()
        {
            var differentPassword = new IdentityFaker().Password;
            _hasherMock.Setup(h => h.VerifyHashedPassword(_user, _faker.PasswordHash.Value, differentPassword.Value))
                .Returns(PasswordVerificationResult.Failed);

            var result = await _passwordService.VerifyAsync(differentPassword, _faker.PasswordHash, CancellationToken.None);

            Assert.AreEqual(PasswordVerifyResult.Failed, result);
        }

        [Test]
        [TestCase(PasswordVerificationResult.Success)]
        [TestCase(PasswordVerificationResult.SuccessRehashNeeded)]
        public async Task VerifyAsync_ForSuccessVerification_ReturnsSuccessResult(PasswordVerificationResult success)
        {
            _hasherMock.Setup(h => h.VerifyHashedPassword(_user, _faker.PasswordHash.Value, _faker.Password.Value))
                .Returns(success);

            var result = await _passwordService.VerifyAsync(_faker.Password, _faker.PasswordHash, CancellationToken.None);

            Assert.AreEqual(PasswordVerifyResult.Success, result);
        }
    }
}

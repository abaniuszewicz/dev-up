using System.Threading;
using System.Threading.Tasks;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Services.Enums;
using DevUp.Domain.Identity.ValueObjects;
using Microsoft.AspNetCore.Identity;
using Moq;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.Services
{
    public class PasswordServiceTests
    {
        private static readonly User _user = null;
        private static readonly CancellationToken CancellationToken = CancellationToken.None;
        private static readonly Password _password = new Password("lowercaseUPPERCASE#1");
        private static readonly PasswordHash _hash = new PasswordHash("r4nd0m-h4$h");

        private Mock<IPasswordHasher<User>> _hasherMock;
        private PasswordService _passwordService;

        [SetUp]
        public void SetUp()
        {
            _hasherMock = new Mock<IPasswordHasher<User>>();
            _passwordService = new PasswordService(_hasherMock.Object);
        }

        [Test]
        public async Task HashAsync_WhenCalled_ForwardsRequestToHasherAndReturnsItsResult()
        {
            _hasherMock.Setup(h => h.HashPassword(_user, _password.Value)).Returns(_hash.Value);

            var hash = await _passwordService.HashAsync(_password, CancellationToken);

            Assert.AreEqual(_hash, hash);
            _hasherMock.Verify(h => h.HashPassword(_user, _password.Value));
        }

        [Test]
        public async Task VerifyAsync_ForFailedVerification_ReturnsFailedResult()
        {
            _hasherMock.Setup(h => h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).Returns(PasswordVerificationResult.Failed);

            var result = await _passwordService.VerifyAsync(_password, _hash, CancellationToken);

            Assert.AreEqual(PasswordVerifyResult.Failed, result);
        }

        [Test]
        [TestCase(PasswordVerificationResult.Success)]
        [TestCase(PasswordVerificationResult.SuccessRehashNeeded)]
        public async Task VerifyAsync_ForSuccessVerification_ReturnsSuccessResult(PasswordVerificationResult success)
        {
            _hasherMock.Setup(h => h.VerifyHashedPassword(It.IsAny<User>(), It.IsAny<string>(), It.IsAny<string>())).Returns(success);

            var result = await _passwordService.VerifyAsync(_password, _hash, CancellationToken);

            Assert.AreEqual(PasswordVerifyResult.Success, result);
        }
    }
}

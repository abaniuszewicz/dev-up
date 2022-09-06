using DevUp.Domain.Identity;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.ValueObjects;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity
{
    public class IdentityResultTests
    {
        [Test]
        public void Constructor_WhenCalled_AssignsProperties()
        {
            var token = new Token("header.payload.signature");
            var refreshToken = new RefreshToken("--valid_refresh_token--");
            var result = new IdentityResult(token, refreshToken);

            Assert.AreEqual(token, result.Token);
            Assert.AreEqual(refreshToken, result.RefreshToken);
        }
    }
}

using DevUp.Domain.Identity.ValueObjects;
using NUnit.Framework;

namespace DevUp.Domain.Tests.Unit.Identity.ValueObjects
{
    public class TokenPairTests
    {
        [Test]
        public void Constructor_WhenCalled_AssignsProperties()
        {
            var token = new Token("header.payload.signature");
            var refreshToken = new RefreshToken("--valid_refresh_token--");
            var result = new TokenPair(token, refreshToken);

            Assert.AreEqual(token, result.Token);
            Assert.AreEqual(refreshToken, result.RefreshToken);
        }
    }
}

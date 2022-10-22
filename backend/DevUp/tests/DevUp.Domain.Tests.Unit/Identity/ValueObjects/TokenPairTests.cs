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

        [Test]
        public void Equality_WhenCompared_ChecksByValue()
        {
            var token1 = new Token("header.payload.signature");
            var refreshToken1 = new RefreshToken("--valid_refresh_token--");
            var tokenPair1 = new TokenPair(token1, refreshToken1);

            var token2 = new Token("header.payload.signature");
            var refreshToken2 = new RefreshToken("--valid_refresh_token--");
            var tokenPair2 = new TokenPair(token2, refreshToken2);

            Assert.AreEqual(tokenPair1, tokenPair2);
        }
    }
}

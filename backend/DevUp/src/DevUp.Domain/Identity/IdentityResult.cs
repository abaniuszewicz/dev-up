using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity
{
    public class IdentityResult
    {
        public Token Token { get; }
        public RefreshToken RefreshToken { get; }

        public IdentityResult(Token token, RefreshToken refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }
    }
}

using DevUp.Domain.Identity.Entities;

namespace DevUp.Domain.Identity.ValueObjects
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

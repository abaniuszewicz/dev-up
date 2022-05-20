using System.Collections.Generic;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public class IdentityResult : ValueObject
    {
        public Token Token { get; }
        public RefreshToken RefreshToken { get; }

        public IdentityResult(Token token, RefreshToken refreshToken)
        {
            Token = token;
            RefreshToken = refreshToken;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Token;
            yield return RefreshToken;
        }
    }
}

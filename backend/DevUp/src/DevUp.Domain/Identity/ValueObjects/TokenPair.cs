using System.Collections.Generic;
using DevUp.Domain.Identity.Entities;
using DevUp.Domain.Identity.Entities.Exceptions;
using DevUp.Domain.Identity.ValueObjects.Exceptions;
using DevUp.Domain.Seedwork;

namespace DevUp.Domain.Identity.ValueObjects
{
    public sealed class TokenPair : ValueObject
    {
        public Token Token { get; }
        public RefreshToken RefreshToken { get; }

        public TokenPair(Token token, RefreshToken refreshToken)
        {
            Validate(token, refreshToken);
            Token = token;
            RefreshToken = refreshToken;
        }

        private void Validate(Token token, RefreshToken refreshToken)
        {
            if (token is null)
                throw new EmptyTokenException();
            if (refreshToken is null)
                throw new EmptyRefreshTokenException();
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Token;
            yield return RefreshToken;
        }
    }
}

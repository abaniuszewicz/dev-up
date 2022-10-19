using System;
using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public class TokenDescriptionException : IdentityBusinessRuleValidationException
    {
        public Token Token { get; }
        public Exception Exception { get; }

        public TokenDescriptionException(Token token, Exception exception) 
            : base("Failed to describe token.")
        {
            Token = token;
            Exception = exception;
        }
    }
}

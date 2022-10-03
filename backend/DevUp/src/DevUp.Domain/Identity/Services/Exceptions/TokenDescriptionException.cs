using System;
using DevUp.Domain.Identity.Exceptions;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public class TokenDescriptionException : IdentityValidationException
    {
        public Exception Exception { get; }

        public TokenDescriptionException(Exception exception) 
            : base("Failed to describe token.")
        {
            Exception = exception;
        }
    }
}

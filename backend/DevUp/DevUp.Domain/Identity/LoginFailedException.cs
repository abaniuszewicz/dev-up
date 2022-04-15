using System;
using System.Collections.Generic;

namespace DevUp.Domain.Identity
{
    public class LoginFailedException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public LoginFailedException(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
}

using System;
using System.Collections.Generic;

namespace DevUp.Domain.Identity.Exceptions
{
    public abstract class IdentityException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public IdentityException(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
}

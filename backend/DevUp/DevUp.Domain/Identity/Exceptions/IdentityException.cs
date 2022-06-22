using System;
using System.Collections.Generic;

namespace DevUp.Domain.Identity.Exceptions
{
    public class IdentityException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public IdentityException(string error)
            : this(new[] { error })
        {
        }

        public IdentityException(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
}

using System;
using System.Collections.Generic;

namespace DevUp.Domain.Seedwork.Exceptions
{
    public abstract class DomainException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public DomainException(string error)
            : this(new[] { error })
        {
        }

        public DomainException(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
}

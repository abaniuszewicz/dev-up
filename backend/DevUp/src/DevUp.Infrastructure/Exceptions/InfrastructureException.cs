using System;
using System.Collections.Generic;

namespace DevUp.Infrastructure.Exceptions
{
    public class InfrastructureException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public InfrastructureException(string error)
            : this(new[] { error })
        {
        }

        public InfrastructureException(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
}

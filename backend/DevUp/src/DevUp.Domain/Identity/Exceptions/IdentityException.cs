using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.Exceptions
{
    public abstract class IdentityException : DomainException
    {
        public IdentityException(string error)
            : base(error)
        {
        }

        public IdentityException(IEnumerable<string> errors)
            : base(errors)
        {
        }
    }
}

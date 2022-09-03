using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.Exceptions
{
    public class IdentityException : DomainException
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

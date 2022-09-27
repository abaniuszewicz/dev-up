using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.Exceptions
{
    public abstract class IdentityNotFoundException : DomainNotFoundException
    {
        public IdentityNotFoundException(string error) 
            : base(error)
        {
        }

        public IdentityNotFoundException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

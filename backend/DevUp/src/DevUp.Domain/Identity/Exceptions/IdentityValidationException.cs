using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.Exceptions
{
    public abstract class IdentityValidationException : DomainValidationException
    {
        public IdentityValidationException(string error) 
            : base(error)
        {
        }

        public IdentityValidationException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

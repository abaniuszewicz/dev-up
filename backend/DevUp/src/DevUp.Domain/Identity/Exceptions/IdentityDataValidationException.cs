using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.Exceptions
{
    public abstract class IdentityDataValidationException : DomainDataValidationException
    {
        public IdentityDataValidationException(string error) 
            : base(error)
        {
        }

        public IdentityDataValidationException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

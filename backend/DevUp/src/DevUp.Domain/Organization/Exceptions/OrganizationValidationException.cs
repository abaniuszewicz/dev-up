using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Organization.Exceptions
{
    public abstract class OrganizationValidationException : DomainValidationException
    {
        protected OrganizationValidationException(string error) 
            : base(error)
        {
        }

        protected OrganizationValidationException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

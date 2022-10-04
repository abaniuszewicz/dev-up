using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Organization.Exceptions
{
    public abstract class OrganizationDataValidationException : DomainDataValidationException
    {
        protected OrganizationDataValidationException(string error) 
            : base(error)
        {
        }

        protected OrganizationDataValidationException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

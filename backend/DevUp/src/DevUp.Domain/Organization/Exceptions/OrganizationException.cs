using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Organization.Exceptions
{
    public abstract class OrganizationException : DomainException
    {
        protected OrganizationException(string error) 
            : base(error)
        {
        }

        protected OrganizationException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

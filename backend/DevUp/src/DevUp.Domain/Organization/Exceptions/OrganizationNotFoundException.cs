using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Organization.Exceptions
{
    public class OrganizationNotFoundException : DomainNotFoundException
    {
        public OrganizationNotFoundException(string error) 
            : base(error)
        {
        }

        public OrganizationNotFoundException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

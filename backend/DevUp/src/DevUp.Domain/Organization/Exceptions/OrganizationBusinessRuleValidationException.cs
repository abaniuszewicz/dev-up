using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Organization.Exceptions
{
    public abstract class OrganizationBusinessRuleValidationException : DomainBusinessRuleValidationException, IOrganizationException
    {
        protected OrganizationBusinessRuleValidationException(string error) 
            : base(error)
        {
        }

        protected OrganizationBusinessRuleValidationException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

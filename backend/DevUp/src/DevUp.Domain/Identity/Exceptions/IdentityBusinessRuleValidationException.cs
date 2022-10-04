using System.Collections.Generic;
using DevUp.Domain.Seedwork.Exceptions;

namespace DevUp.Domain.Identity.Exceptions
{
    public abstract class IdentityBusinessRuleValidationException : DomainBusinessRuleValidationException
    {
        protected IdentityBusinessRuleValidationException(string error) 
            : base(error)
        {
        }

        protected IdentityBusinessRuleValidationException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

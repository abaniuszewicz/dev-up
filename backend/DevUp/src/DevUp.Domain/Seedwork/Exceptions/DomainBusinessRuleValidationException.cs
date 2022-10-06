using System.Collections.Generic;

namespace DevUp.Domain.Seedwork.Exceptions
{
    public abstract class DomainBusinessRuleValidationException : DomainException
    {
        protected DomainBusinessRuleValidationException(string error) 
            : base(error)
        {
        }

        protected DomainBusinessRuleValidationException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

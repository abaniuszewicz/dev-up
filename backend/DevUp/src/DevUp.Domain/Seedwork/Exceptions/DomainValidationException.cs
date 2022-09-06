using System.Collections.Generic;

namespace DevUp.Domain.Seedwork.Exceptions
{
    public class DomainValidationException : DomainException
    {
        public DomainValidationException(string error)
            : base(error)
        {
        }

        public DomainValidationException(IEnumerable<string> errors)
            : base(errors)
        {
        }
    }
}

using System.Collections.Generic;

namespace DevUp.Domain.Seedwork.Exceptions
{
    public class DomainDataValidationException : DomainException
    {
        public DomainDataValidationException(string error)
            : base(error)
        {
        }

        public DomainDataValidationException(IEnumerable<string> errors)
            : base(errors)
        {
        }
    }
}

using System.Collections.Generic;

namespace DevUp.Domain.Seedwork.Exceptions
{
    public class ValidationException : DomainException
    {
        public ValidationException(string error)
            : base(error)
        {
        }

        public ValidationException(IEnumerable<string> errors)
            : base(errors)
        {
        }
    }
}

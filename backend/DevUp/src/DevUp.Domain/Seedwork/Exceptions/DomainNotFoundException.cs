using System.Collections.Generic;

namespace DevUp.Domain.Seedwork.Exceptions
{
    public class DomainNotFoundException : DomainException
    {
        public DomainNotFoundException(string error) 
            : base(error)
        {
        }

        public DomainNotFoundException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

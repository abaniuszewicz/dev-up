using System.Collections.Generic;

namespace DevUp.Domain.Identity.Exceptions
{
    public class RegistrationFailedException
        : IdentityException
    {
        public RegistrationFailedException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

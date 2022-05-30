using System.Collections.Generic;

namespace DevUp.Domain.Identity.Exceptions
{
    public class RefreshFailedException : IdentityException
    {
        public RefreshFailedException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Identity;

namespace DevUp.Domain.Identity
{
    public class RegistrationFailedException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public RegistrationFailedException(IEnumerable<IdentityError> errors)
            : this(errors.Select(e => e.Description))
        {
        }

        public RegistrationFailedException(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
}

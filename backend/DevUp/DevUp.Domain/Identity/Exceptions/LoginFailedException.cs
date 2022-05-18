﻿using System.Collections.Generic;

namespace DevUp.Domain.Identity.Exceptions
{
    public class LoginFailedException : IdentityException
    {
        public LoginFailedException(IEnumerable<string> errors) 
            : base(errors)
        {
        }
    }
}

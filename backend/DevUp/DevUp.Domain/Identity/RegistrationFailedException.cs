﻿using System;
using System.Collections.Generic;

namespace DevUp.Domain.Identity
{
    public class RegistrationFailedException : Exception
    {
        public IEnumerable<string> Errors { get; }

        public RegistrationFailedException(IEnumerable<string> errors)
        {
            Errors = errors;
        }
    }
}

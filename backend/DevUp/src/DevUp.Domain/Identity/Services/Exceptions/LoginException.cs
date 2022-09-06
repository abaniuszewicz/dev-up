﻿using System.Collections.Generic;

namespace DevUp.Domain.Identity.Exceptions
{
    internal sealed class LoginException : IdentityValidationException
    {
        internal const string InvalidUsernameMessage = "User with this username does not exists.";
        internal const string InvalidPasswordMessage = "Invalid password.";
        internal const string HashNotFoundMessage = "Failed to retrieve password hash.";

        public LoginException(string error) : base(error)
        {
        }

        public LoginException(IEnumerable<string> errors) : base(errors)
        {
        }
    }
}
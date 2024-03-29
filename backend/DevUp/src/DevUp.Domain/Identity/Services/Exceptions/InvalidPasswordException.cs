﻿using DevUp.Domain.Identity.Exceptions;
using DevUp.Domain.Identity.ValueObjects;

namespace DevUp.Domain.Identity.Services.Exceptions
{
    public class InvalidPasswordException : IdentityBusinessRuleValidationException
    {
        public Username Username { get; }

        public InvalidPasswordException(Username username) 
            : base("Supplied password does not match password of user with this username.")
        {
            Username = username;
        }
    }
}

﻿using DevUp.Domain.Organization.Exceptions;

namespace DevUp.Domain.Organization.ValueObjects.Exceptions
{
    public class EmptyRoleException : OrganizationDataValidationException
    {
        public EmptyRoleException() 
            : base("Role cannot be empty.")
        {
        }
    }
}

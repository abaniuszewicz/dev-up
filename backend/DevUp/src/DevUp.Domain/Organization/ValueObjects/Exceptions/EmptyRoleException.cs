using DevUp.Domain.Organization.Exceptions;

namespace DevUp.Domain.Organization.ValueObjects.Exceptions
{
    public class EmptyRoleException : OrganizationValidationException
    {
        public EmptyRoleException() 
            : base("Role cannot be empty.")
        {
        }
    }
}

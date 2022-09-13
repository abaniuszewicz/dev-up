using DevUp.Domain.Organization.Exceptions;

namespace DevUp.Domain.Organization.ValueObjects
{
    public class EmptyMemberNameException : OrganizationValidationException
    {
        public EmptyMemberNameException()
            : base("Member name cannot be empty.")
        {
        }
    }
}

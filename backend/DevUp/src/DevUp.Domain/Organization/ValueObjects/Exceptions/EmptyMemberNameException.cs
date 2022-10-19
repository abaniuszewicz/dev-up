using DevUp.Domain.Organization.Exceptions;

namespace DevUp.Domain.Organization.ValueObjects.Exceptions
{
    public class EmptyMemberNameException : OrganizationDataValidationException
    {
        public EmptyMemberNameException()
            : base("Member name cannot be empty.")
        {
        }
    }
}

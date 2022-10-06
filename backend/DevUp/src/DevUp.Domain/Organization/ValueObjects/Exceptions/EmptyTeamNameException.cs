using DevUp.Domain.Organization.Exceptions;

namespace DevUp.Domain.Organization.ValueObjects.Exceptions
{
    public sealed class EmptyTeamNameException : OrganizationDataValidationException
    {
        public EmptyTeamNameException() 
            : base("Team name cannot be empty.")
        {
        }
    }
}

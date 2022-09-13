using DevUp.Domain.Organization.Exceptions;

namespace DevUp.Domain.Organization.ValueObjects.Exceptions
{
    public sealed class EmptyTeamNameException : OrganizationValidationException
    {
        public EmptyTeamNameException() 
            : base("Team name cannot be empty.")
        {
        }
    }
}

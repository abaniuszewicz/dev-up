using DevUp.Domain.Organization.Exceptions;
using DevUp.Domain.Organization.ValueObjects;

namespace DevUp.Domain.Organization.Services.Exceptions
{
    public sealed class TeamNameAlreadyTakenException : OrganizationValidationException
    {
        public TeamName Name { get; }

        public TeamNameAlreadyTakenException(TeamName name) 
            : base("Team with this name already exists.")
        {
            Name = name;
        }
    }
}

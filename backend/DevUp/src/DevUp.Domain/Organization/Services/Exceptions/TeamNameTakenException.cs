using DevUp.Domain.Organization.Exceptions;
using DevUp.Domain.Organization.ValueObjects;

namespace DevUp.Domain.Organization.Services.Exceptions
{
    public sealed class TeamNameTakenException : OrganizationBusinessRuleValidationException
    {
        public TeamName Name { get; }

        public TeamNameTakenException(TeamName name) 
            : base("Team with this name already exists.")
        {
            Name = name;
        }
    }
}

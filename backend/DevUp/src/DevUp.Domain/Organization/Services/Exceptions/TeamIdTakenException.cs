using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Exceptions;

namespace DevUp.Domain.Organization.Services.Exceptions
{
    public sealed class TeamIdTakenException : OrganizationBusinessRuleValidationException
    {
        public TeamId Id { get; }

        public TeamIdTakenException(TeamId id)
            : base("Team with this id already exists.")
        {
            Id = id;
        }
    }
}

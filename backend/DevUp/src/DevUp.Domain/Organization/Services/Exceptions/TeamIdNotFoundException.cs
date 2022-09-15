using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Exceptions;

namespace DevUp.Domain.Organization.Services.Exceptions
{
    public sealed class TeamIdNotFoundException : OrganizationValidationException
    {
        public TeamId Id { get; }

        public TeamIdNotFoundException(TeamId id)
            : base("Team with this id does not exist.")
        {
            Id = id;
        }
    }
}

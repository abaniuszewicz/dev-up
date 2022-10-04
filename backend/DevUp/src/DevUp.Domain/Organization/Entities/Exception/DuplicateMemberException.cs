using DevUp.Domain.Organization.Exceptions;

namespace DevUp.Domain.Organization.Entities.Exception
{
    public sealed class DuplicateMemberException : OrganizationBusinessRuleValidationException
    {
        public TeamId TeamId { get; }
        public MemberId MemberId { get; }

        public DuplicateMemberException(TeamId teamId, MemberId memberId)
            : base($"Member {memberId} is already part of team {teamId}.")
        {
            TeamId = teamId;
            MemberId = memberId;
        }
    }
}

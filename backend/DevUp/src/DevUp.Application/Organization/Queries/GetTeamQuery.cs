using DevUp.Application.Organization.Queries.Results;
using MediatR;

namespace DevUp.Application.Organization.Queries
{
    public class GetTeamQuery : IRequest<TeamQueryResult>
    {
        public Guid Id { get; init; }
    }
}

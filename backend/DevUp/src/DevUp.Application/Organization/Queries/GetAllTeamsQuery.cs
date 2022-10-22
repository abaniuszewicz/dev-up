using DevUp.Application.Organization.Queries.Results;
using MediatR;

namespace DevUp.Application.Organization.Queries
{
    public sealed class GetAllTeamsQuery : IRequest<IEnumerable<TeamQueryResult>>
    {
    }
}

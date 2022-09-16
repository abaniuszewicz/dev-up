using System.Data;
using AutoMapper;
using Dapper;
using DevUp.Application.Organization.Queries;
using DevUp.Application.Organization.Queries.Results;
using DevUp.Domain.Common.Extensions;
using DevUp.Infrastructure.Postgres.Organization.Dtos;
using DevUp.Infrastructure.Postgres.Setup;
using MediatR;

namespace DevUp.Infrastructure.Postgres.Organization.Queries.Handlers
{
    internal class GetAllTeamsQueryHandler : IRequestHandler<GetAllTeamsQuery, IEnumerable<TeamQueryResult>>
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;

        public GetAllTeamsQueryHandler(IDbConnectionFactory connectionFactory, IMapper mapper)
        {
            _connection = connectionFactory.Create(DbConnectionName.Organization);
            _mapper = mapper;
        }

        public async Task<IEnumerable<TeamQueryResult>> Handle(GetAllTeamsQuery request, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (request is null)
                throw new ArgumentNullException(nameof(request));

            var sql = @$"SELECT 
                            id {nameof(TeamDto.Id)}, 
                            name {nameof(TeamDto.Name)}
                        FROM teams";

            var dtos = await _connection.QueryAsync<TeamDto>(sql);
            return dtos.Select(_mapper.MapOrNull<TeamQueryResult>).ToList();
        }
    }
}

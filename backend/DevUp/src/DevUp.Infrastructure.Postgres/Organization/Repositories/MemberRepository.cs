using System.Data;
using AutoMapper;
using Dapper;
using DevUp.Domain.Common.Extensions;
using DevUp.Domain.Organization.Entities;
using DevUp.Domain.Organization.Repositories;
using DevUp.Infrastructure.Postgres.Organization.Dtos;
using DevUp.Infrastructure.Postgres.Setup;

namespace DevUp.Infrastructure.Postgres.Organization.Repositories
{
    internal class MemberRepository : IMemberRepository
    {
        private readonly IDbConnection _connection;
        private readonly IMapper _mapper;

        public MemberRepository(IDbConnectionFactory connectionFactory, IMapper mapper)
        {
            _connection = connectionFactory.Create(DbConnectionName.Organization);
            _mapper = mapper;
        }

        public async Task<MemberInvitation?> CreateAsync(MemberInvitation memberInvitation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (memberInvitation is null)
                throw new ArgumentNullException(nameof(memberInvitation));

            var dto = _mapper.Map<MemberInvitationDto>(memberInvitation);
            var sql = $@"INSERT INTO members_invitations (
                            sender_id,
                            receiver_id,
                            team_id
                         )
                         VALUES (
                            @{nameof(MemberInvitationDto.SenderId)},
                            @{nameof(MemberInvitationDto.ReceiverId)},
                            @{nameof(MemberInvitationDto.TeamId)}
                         )";

            var affectedRows = await _connection.ExecuteAsync(sql, dto);
            return affectedRows == 0 ? null : memberInvitation;
        }

        public async Task<Member> GetByIdAsync(MemberInvitation memberInvitation, CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();
            if (memberInvitation is null)
                throw new ArgumentNullException(nameof(memberInvitation));

            var dto = new MemberDto() { UserId = memberInvitation.Id.ReceiverId, TeamId = memberInvitation.TeamId };
            var sql = $@"SELECT
                            user_id {nameof(MemberDto.UserId)},
                            team_id {nameof(MemberDto.TeamId)}
                         FROM members
                         WHERE user_id = @{nameof(MemberDto.UserId)} AND team_id = @{nameof(MemberDto.TeamId)}";

            dto = await _connection.QuerySingleOrDefaultAsync<MemberDto>(sql, dto);
            return _mapper.MapOrNull<Member>(dto);
        }
    }
}

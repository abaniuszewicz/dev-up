using FluentMigrator;

namespace DevUp.Infrastructure.Postgres.Migrations
{
    [Migration(2022092200002)]
    public class AddMembersInvitationTable_2022092200002 : Migration
    {
        public override void Up()
        {
            Create.Table("members_invitations")
                .WithColumn("sender_id")
                    .AsGuid()
                    .ForeignKey("users", "id")
                .WithColumn("receiver_id")
                    .AsGuid()
                    .ForeignKey("users", "id")
                .WithColumn("team_id")
                    .AsGuid()
                    .ForeignKey("teams", "id");
            //.WithColumn("valid_due")
            //    .AsDateTime();

            Create.UniqueConstraint()
                .OnTable("members_invitations")
                .Columns("sender_id", "receiver_id", "team_id");
        }

        public override void Down()
        {
            Delete.Table("members_invitations");
        }
    }
}

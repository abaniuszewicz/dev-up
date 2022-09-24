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
                    .PrimaryKey()
                .WithColumn("receiver_id")
                    .AsGuid()
                    .ForeignKey("users", "id")
                    .PrimaryKey()
                .WithColumn("team_id")
                    .AsGuid()
                    .ForeignKey("teams", "id")
                    .PrimaryKey();
                //.WithColumn("valid_due")
                //    .AsDateTime();
        }

        public override void Down()
        {
            Delete.Table("members_invitations");
        }
    }
}

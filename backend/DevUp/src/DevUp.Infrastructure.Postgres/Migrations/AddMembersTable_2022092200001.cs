using FluentMigrator;

namespace DevUp.Infrastructure.Postgres.Migrations
{
    [Migration(2022092200001)]
    public class AddMembersTable_2022092200001 : Migration
    {
        public override void Up()
        {
            Create.Table("members")
                .WithColumn("user_id")
                    .AsGuid()
                    .NotNullable()
                    .ForeignKey("users", "id")
                .WithColumn("team_id")
                    .AsGuid()
                    .NotNullable()
                    .ForeignKey("teams", "id");

            Create.UniqueConstraint()
                .OnTable("members")
                .Columns("user_id", "team_id");
        }

        public override void Down()
        {
            Delete.Table("members");
        }
    }
}

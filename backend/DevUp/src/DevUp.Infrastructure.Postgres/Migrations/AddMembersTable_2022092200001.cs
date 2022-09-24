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
                    .PrimaryKey()
                .WithColumn("team_id")
                    .AsGuid()
                    .NotNullable()
                    .ForeignKey("teams", "id")
                    .PrimaryKey();
        }

        public override void Down()
        {
            Delete.Table("members");
        }
    }
}

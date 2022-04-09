using FluentMigrator;

namespace DevUp.Infrastructure.Postgres.Migrations
{
    [Migration(2022040700001)]
    public class AddUsersTable_2022040700001 : Migration
    {
        public override void Up()
        {
            Create.Table("Users")
                .WithColumn("Username").AsAnsiString().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}

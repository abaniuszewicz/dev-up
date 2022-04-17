using FluentMigrator;

namespace DevUp.Infrastructure.Postgres.Migrations
{
    [Migration(2022040700001)]
    public class AddUsersTable_2022040700001 : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("userName").AsAnsiString().NotNullable()
                .WithColumn("passwordHash").AsAnsiString();
        }

        public override void Down()
        {
            Delete.Table("users");
        }
    }
}

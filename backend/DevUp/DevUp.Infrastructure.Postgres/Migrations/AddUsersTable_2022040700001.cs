using FluentMigrator;

namespace DevUp.Infrastructure.Postgres.Migrations
{
    [Migration(2022040700001)]
    public class AddUsersTable_2022040700001 : Migration
    {
        public override void Up()
        {
            Create.Table("users")
                .WithColumn("Id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("UserName").AsAnsiString().NotNullable()
                .WithColumn("PasswordHash").AsAnsiString();
        }

        public override void Down()
        {
            Delete.Table("Users");
        }
    }
}

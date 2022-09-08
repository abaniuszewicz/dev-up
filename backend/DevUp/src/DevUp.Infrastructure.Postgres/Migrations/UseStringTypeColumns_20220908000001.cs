using FluentMigrator;

namespace DevUp.Infrastructure.Postgres.Migrations
{
    [Migration(2022090800001)]
    public class UseStringTypeColumns_2022090800001 : Migration
    {
        public override void Up()
        {
            Alter.Table("users")
                .AlterColumn("username").AsString()
                .AlterColumn("password_hash").AsString();
            Alter.Table("refresh_tokens")
                .AlterColumn("token").AsString();

        }

        public override void Down()
        {
            Alter.Table("refresh_tokens")
                .AlterColumn("token").AsAnsiString();
            Alter.Table("users")
                .AlterColumn("username").AsAnsiString()
                .AlterColumn("password_hash").AsAnsiString();
        }
    }
}

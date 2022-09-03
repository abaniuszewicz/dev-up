using FluentMigrator;

namespace DevUp.Infrastructure.Postgres.Migrations
{
    [Migration(2022052600001)]
    public class DeleteColumnFromUsersTable_2022052600001 : Migration
    {
        public override void Up()
        {
            Delete.Column("normalized_username").FromTable("users");
        }

        public override void Down()
        {
            Create.Column("normalized_username").OnTable("users").AsString();
            var sql = "UPDATE users SET normalized_username = UPPER(username)";
            Execute.Sql(sql);
        }
    }
}

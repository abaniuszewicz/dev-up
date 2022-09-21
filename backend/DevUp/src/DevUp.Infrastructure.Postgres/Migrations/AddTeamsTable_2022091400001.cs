using FluentMigrator;

namespace DevUp.Infrastructure.Postgres.Migrations
{
    [Migration(2022091400001)]
    public class AddTeamsTable_2022091400001 : Migration
    {
        public override void Up()
        {
            Create.Table("teams")
                .WithColumn("id").AsGuid().NotNullable().PrimaryKey()
                .WithColumn("name").AsString().NotNullable();
        }

        public override void Down()
        {
            Delete.Table("teams");
        }
    }
}

using FluentMigrator;

namespace DevUp.Infrastructure.Postgres.Migrations
{
    [Migration(2022050100002)]
    public class AddDevicesTable_2022051600002 : Migration
    {
        public override void Up()
        {
            Create.Table("devices")
                .WithColumn("id").AsString().NotNullable().PrimaryKey()
                .WithColumn("name").AsString();

            Create.ForeignKey()
                .FromTable("refresh_tokens").ForeignColumn("device_id")
                .ToTable("devices").PrimaryColumn("id");
        }

        public override void Down()
        {
            Delete.ForeignKey()
                .FromTable("refresh_tokens").ForeignColumn("device_id")
                .ToTable("devices").PrimaryColumn("id");

            Delete.Table("devices");
        }
    }
}

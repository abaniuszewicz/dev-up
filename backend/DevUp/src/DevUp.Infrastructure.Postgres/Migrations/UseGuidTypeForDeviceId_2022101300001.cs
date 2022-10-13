using FluentMigrator;

namespace DevUp.Infrastructure.Postgres.Migrations
{
    [Migration(2022101300001)]
    public class UseGuidTypeForDeviceId_2022101300001 : Migration
    {
        public override void Up()
        {
            DeleteConstraints();

            void AsGuid(string table, string column) 
                => Execute.Sql($"ALTER TABLE {table} ALTER COLUMN {column} TYPE uuid USING {column}::uuid");
            AsGuid("devices", "id");
            AsGuid("refresh_tokens", "device_id");

            RestoreConstraints();
        }

        public override void Down()
        {
            DeleteConstraints();

            Alter.Table("devices").AlterColumn("id").AsString();
            Alter.Table("refresh_tokens").AlterColumn("device_id").AsString();

            RestoreConstraints();
        }

        private void DeleteConstraints()
        {
            Delete.ForeignKey()
                .FromTable("refresh_tokens").ForeignColumn("device_id")
                .ToTable("devices").PrimaryColumn("id");
            Delete.PrimaryKey("PK_devices")
                .FromTable("devices");
        }

        private void RestoreConstraints()
        {
            Create.PrimaryKey("PK_devices")
                .OnTable("devices")
                .Column("id");
            Create.ForeignKey()
                .FromTable("refresh_tokens").ForeignColumn("device_id")
                .ToTable("devices").PrimaryColumn("id");
        }
    }
}

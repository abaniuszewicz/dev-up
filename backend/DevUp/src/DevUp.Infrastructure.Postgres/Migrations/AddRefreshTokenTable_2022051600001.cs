using FluentMigrator;

namespace DevUp.Infrastructure.Postgres.Migrations
{
    [Migration(2022050100001)]
    public class AddRefreshTokenTable_2022051600001 : Migration
    {
        public override void Up()
        {
            Create.Table("refresh_tokens")
                .WithColumn("token").AsAnsiString().PrimaryKey()
                .WithColumn("jti").AsString().NotNullable()
                .WithColumn("user_id").AsGuid()
                .WithColumn("creation_date").AsDateTime2()
                .WithColumn("expiry_date").AsDateTime2()
                .WithColumn("device_id").AsString()
                .WithColumn("used").AsBoolean()
                .WithColumn("invalidated").AsBoolean();

            Create.ForeignKey()
                .FromTable("refresh_tokens").ForeignColumn("user_id")
                .ToTable("users").PrimaryColumn("id");
        }

        public override void Down()
        {
            Delete.ForeignKey()
                .FromTable("refresh_tokens").ForeignColumn("user_id")
                .ToTable("users").PrimaryColumn("id");

            Delete.Table("refresh_tokens");
        }
    }
}

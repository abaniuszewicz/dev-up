using FluentMigrator;

namespace DevUp.Infrastructure.Postgres.Migrations
{
    [Migration(2022101000001)]
    public class AddChainColumnsToRefreshTokenTable_2022101000001 : Migration
    {
        public override void Up()
        {
            Alter.Table("refresh_tokens")
                .AddColumn("previous").AsAnsiString().Nullable()
                .AddColumn("next").AsAnsiString().Nullable();

            Create.ForeignKey()
                .FromTable("refresh_tokens").ForeignColumn("previous")
                .ToTable("refresh_tokens").PrimaryColumn("token");
            Create.ForeignKey()
                .FromTable("refresh_tokens").ForeignColumn("next")
                .ToTable("refresh_tokens").PrimaryColumn("token");
        }

        public override void Down()
        {
            Delete.ForeignKey()
                .FromTable("refresh_tokens").ForeignColumn("next")
                .ToTable("refresh_tokens").PrimaryColumn("token");
            Delete.ForeignKey()
                .FromTable("refresh_tokens").ForeignColumn("previous")
                .ToTable("refresh_tokens").PrimaryColumn("token");

            Delete.Column("next").FromTable("refresh_tokens");
            Delete.Column("previous").FromTable("refresh_tokens");
        }
    }
}

using DevUp.Infrastructure.Postgres.Setup;
using FluentMigrator.Runner;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DevUp.Infrastructure.Postgres.Migrations
{
    public static class FluentMigratorInstaller
    {
        public static IServiceCollection AddDatabaseMigrator(this IServiceCollection services)
        {
            return services.AddFluentMigratorCore()
                .ConfigureRunner(mrb =>
                    mrb.AddPostgres()
                        .WithGlobalConnectionString(sp => sp.GetService<IDbConnectionFactory>()!.GetConnectionString(DbConnectionName.Identity))
                        .ScanIn(typeof(FluentMigratorInstaller).Assembly).For.Migrations())
                .AddLogging(lb => lb.AddFluentMigratorConsole());
        }

        public static IHost MigrateUp(this IHost host)
        {
            using var scope = host.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp();
            return host;
        }

        public static IHost MigrateUp(this IHost host, long version)
        {
            using var scope = host.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateUp(version);
            return host;
        }

        public static IHost MigrateDown(this IHost host, long version)
        {
            using var scope = host.Services.CreateScope();
            var runner = scope.ServiceProvider.GetRequiredService<IMigrationRunner>();
            runner.MigrateDown(version);
            return host;
        }
    }
}

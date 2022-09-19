using System.Data;
using Microsoft.Extensions.Options;
using Npgsql;

namespace DevUp.Infrastructure.Postgres.Setup
{
    internal class PostgresConnectionFactory : IDbConnectionFactory
    {
        private readonly PostgresOptions _postgresOptions;

        public PostgresConnectionFactory(IOptions<PostgresOptions> postgresOptions)
        {
            _postgresOptions = postgresOptions.Value;
        }

        public string GetConnectionString(DbConnectionName connectionName)
        {
            return connectionName switch
            {
                DbConnectionName.Identity => _postgresOptions.ConnectionString,
                _ => throw new ArgumentOutOfRangeException(nameof(connectionName))
            };
        }

        public IDbConnection Create(DbConnectionName connectionName)
        {
            var connectionString = GetConnectionString(connectionName);
            return new NpgsqlConnection(connectionString);
        }
    }
}

using System.Data;
using DevUp.Domain.Common.Services;
using Npgsql;

namespace DevUp.Infrastructure.Postgres.Setup
{
    internal class PostgresConnectionFactory : IDbConnectionFactory
    {
        private readonly ISecretProvider _secretProvider;

        public PostgresConnectionFactory(ISecretProvider secretProvider)
        {
            _secretProvider = secretProvider;
        }

        public string GetConnectionString(DbConnectionName connectionName)
        {
            return connectionName switch
            {
                DbConnectionName.Identity => _secretProvider.Get("DB_CONNECTION_STRING"),
                DbConnectionName.Organization => _secretProvider.Get("DB_CONNECTION_STRING"),
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

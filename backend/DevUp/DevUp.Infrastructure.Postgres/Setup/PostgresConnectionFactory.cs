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

        public IDbConnection Create(DbConnectionName connectionName)
        {
            var connectionString = connectionName switch
            {
                DbConnectionName.Identity => _secretProvider.Get("DB_CONNECTION_STRING"),
                _ => throw new ArgumentOutOfRangeException(nameof(connectionName))
            };

            return new NpgsqlConnection(connectionString);
        }
    }
}

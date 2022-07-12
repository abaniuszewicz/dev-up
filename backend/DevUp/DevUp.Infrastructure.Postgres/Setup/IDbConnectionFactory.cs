using System.Data;

namespace DevUp.Infrastructure.Postgres.Setup
{
    public interface IDbConnectionFactory
    {
        public string GetConnectionString(DbConnectionName connectionName);
        public IDbConnection Create(DbConnectionName connectionName);
    }
}

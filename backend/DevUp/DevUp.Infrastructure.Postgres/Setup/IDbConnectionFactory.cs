using System.Data;

namespace DevUp.Infrastructure.Postgres.Setup
{
    public interface IDbConnectionFactory
    {
        public IDbConnection Create(DbConnectionName connectionName);
    }
}

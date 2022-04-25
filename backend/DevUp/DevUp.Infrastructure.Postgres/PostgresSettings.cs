namespace DevUp.Infrastructure.Postgres
{
    internal class PostgresSettings
    {
        public string ConnectionString { get; }

        public PostgresSettings()
        {
            ConnectionString = Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException("Failed to find DB_CONNECTION_STRING definition");
        }
    }
}

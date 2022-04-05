using DevUp.Infrastructure.System;

namespace DevUp.Infrastructure.Persistence.MongoDb
{
    internal class MongoSettings
    {
        private const string ConnectionStringKey = "MONGODB_URL";

        public string ConnectionString { get; }

        public MongoSettings(IEnvironmentVariableRetriever environmentVariableRetriever)
        {
            ConnectionString = environmentVariableRetriever.GetVariable(ConnectionStringKey);
        }
    }
}

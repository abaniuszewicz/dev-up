﻿using System.Data;
using DevUp.Infrastructure.Postgres.Setup;
using DotNet.Testcontainers.Containers;
using Npgsql;

namespace DevUp.Api.Tests.Integration.Common
{
    internal class TestcontainerDbConnectionFactory : IDbConnectionFactory
    {
        private readonly TestcontainerDatabase _database;

        public TestcontainerDbConnectionFactory(TestcontainerDatabase database)
        {
            _database = database;
        }

        public IDbConnection Create(DbConnectionName connectionName)
        {
            var connectionString = GetConnectionString(connectionName);
            return new NpgsqlConnection(connectionString);
        }

        public string GetConnectionString(DbConnectionName connectionName)
        {
            return _database.ConnectionString;
        }
    }
}
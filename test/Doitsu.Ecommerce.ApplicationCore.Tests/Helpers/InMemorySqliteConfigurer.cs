using System;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Doitsu.Ecommerce.Core.Tests.Helpers
{
     public class InMemorySqliteConfigurer : IDatabaseConfigurer
    {
        private readonly SqliteConnection _connection;
        private readonly ILoggerFactory _loggerFactory;

        public InMemorySqliteConfigurer(ILoggerFactory loggerFactory)
        {
            _connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = ":memory:" }.ToString());
            _connection.Open();
            _loggerFactory = loggerFactory;
        }

        public virtual void Configure(DbContextOptionsBuilder builder, string migrationAssembly)
        {
            if (string.IsNullOrEmpty(migrationAssembly)) throw new ArgumentNullException(nameof(migrationAssembly));
            builder.UseSqlite(_connection, builder => builder.MigrationsAssembly(migrationAssembly));
            builder.UseLoggerFactory(_loggerFactory);
            builder.EnableSensitiveDataLogging();
        }
    }
}

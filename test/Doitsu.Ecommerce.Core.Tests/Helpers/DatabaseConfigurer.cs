using System;
using Doitsu.Ecommerce.Core.Tests.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Doitsu.Ecommerce.Core.Tests.Helpers
{
    public abstract class DatabaseConfigurer : IDatabaseConfigurer
    {
        public abstract string ConnectionStringName { get; }
        private readonly string _connectionString;
        private readonly ILoggerFactory _loggerFactory;

        public DatabaseConfigurer(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _connectionString = configuration.GetConnectionString(ConnectionStringName);
            _loggerFactory = loggerFactory;
        }

        public virtual void Configure(DbContextOptionsBuilder builder, string migrationAssembly)
        {
            if (string.IsNullOrEmpty(migrationAssembly))
                throw new ArgumentNullException(nameof(migrationAssembly));
            builder.UseSqlServer(_connectionString,
                builder => builder.MigrationsAssembly(migrationAssembly));
            builder.UseLoggerFactory(_loggerFactory);
        }
    }
}
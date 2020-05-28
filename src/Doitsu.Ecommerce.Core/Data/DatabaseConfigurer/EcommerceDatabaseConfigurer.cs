using System;
using Doitsu.Service.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Doitsu.Ecommerce.Core.Data.DatabaseConfigurer
{
    public class EcommerceDatabaseConfigurer : IDatabaseConfigurer
    {
        public string ConnectionStringName { get => nameof(EcommerceDbContext); }
        private readonly string _connectionString;
        private readonly ILoggerFactory _loggerFactory;

        public EcommerceDatabaseConfigurer(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _connectionString = configuration.GetConnectionString(ConnectionStringName);
            _loggerFactory = loggerFactory;
        }

        public virtual void Configure(DbContextOptionsBuilder builder, string migrationAssembly)
        {
            if (string.IsNullOrEmpty(migrationAssembly)) throw new ArgumentNullException(nameof(migrationAssembly));
            builder.UseSqlServer(_connectionString, builder => builder.MigrationsAssembly(migrationAssembly));
            builder.UseLoggerFactory(_loggerFactory);
            builder.EnableSensitiveDataLogging();
        }
    }
}
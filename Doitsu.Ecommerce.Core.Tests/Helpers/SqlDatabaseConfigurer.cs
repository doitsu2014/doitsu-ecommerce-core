using Doitsu.Ecommerce.Core.Tests.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Doitsu.Ecommerce.Core.Tests.Helpers
{
    public class SqlDatabaseConfigurer : IDatabaseConfigurer
    {
        private readonly string _identityConnectionString;
        private readonly ILoggerFactory _loggerFactory;
        private string _migrationsAssembly;

        public SqlDatabaseConfigurer(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _identityConnectionString = configuration.GetConnectionString("SecurityDbTest");
            _loggerFactory = loggerFactory;
            _migrationsAssembly = typeof(SqlDatabaseConfigurer).Assembly.GetName().Name;
        }

        public void Configure(DbContextOptionsBuilder builder, string assembly)
        {
            builder.UseSqlServer(_identityConnectionString,
                sql => sql.MigrationsAssembly(assembly ?? _migrationsAssembly));
            builder.UseLoggerFactory(_loggerFactory);
        }
    }
}

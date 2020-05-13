using Doitsu.Ecommerce.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Doitsu.Ecommerce.Core.Tests.Helpers
{
    public class TestDatabaseConfigurer : DatabaseConfigurer
    {
        public TestDatabaseConfigurer(IConfiguration configuration, ILoggerFactory loggerFactory)
            : base(configuration, loggerFactory) { }

        public override string ConnectionStringName => nameof(EcommerceDbContext);

        public override void Configure(DbContextOptionsBuilder builder, string migrationAssembly)
        {
            base.Configure(builder, migrationAssembly);
            builder.EnableSensitiveDataLogging();
        }
    }
}

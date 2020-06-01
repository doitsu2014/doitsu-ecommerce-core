using Doitsu.Ecommerce.Core.IdentityServer4.Data;
using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Hris.Data.Identity
{
    public class EcommerceIs4PersistedGrantDbContextFactory : IDesignTimeDbContextFactory<EcommerceIs4PersistedGrantDbContext>
    {
        EcommerceIs4PersistedGrantDbContext IDesignTimeDbContextFactory<EcommerceIs4PersistedGrantDbContext>.CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<PersistedGrantDbContext>();
            var connectionString = "Server=103.114.104.24,1444;Database=factory;Trusted_Connection=False;User Id=sa;Password=zaQ@1234";
            builder.UseSqlServer(connectionString);
            return new EcommerceIs4PersistedGrantDbContext(builder.Options, new OperationalStoreOptions() { });
        }
    }
}

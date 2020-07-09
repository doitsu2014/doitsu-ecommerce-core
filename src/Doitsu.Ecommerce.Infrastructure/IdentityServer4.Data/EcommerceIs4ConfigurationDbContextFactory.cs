using IdentityServer4.EntityFramework.DbContexts;
using IdentityServer4.EntityFramework.Options;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Doitsu.Ecommerce.Infrastructure.IdentityServer4.Data
{
    public class EcommerceIs4ConfigurationDbContextFactory : IDesignTimeDbContextFactory<EcommerceIs4ConfigurationDbContext>
    {
        public EcommerceIs4ConfigurationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ConfigurationDbContext>();

            // this line very important, to config the destination of Database to Migration
            // var connectionString = "Server=localhost,1433;Database=BachMoc_Furniture_Dev;Trusted_Connection=False;User Id=sa;Password=zaQ@1234";
            // var connectionString = "Server=garden.dotvndns.vn,1444;Database=bachmoc_ver2_production;Trusted_Connection=False;User Id=bachmoc;Password=zaQ@1234";
            
            // YGFL
            var connectionString = "Server=103.114.104.24,1444;Database=factory;Trusted_Connection=False;User Id=sa;Password=zaQ@1234";
            builder.UseSqlServer(connectionString);
            return new EcommerceIs4ConfigurationDbContext(builder.Options, new ConfigurationStoreOptions() {  });
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Doitsu.Ecommerce.Infrastructure.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<EcommerceDbContext>
    {
        public EcommerceDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<EcommerceDbContext>();

            // this line very important, to config the destination of Database to Migration
            // var connectionString = "Server=localhost,1433;Database=BachMoc_Furniture_Dev;Trusted_Connection=False;User Id=sa;Password=zaQ@1234";
            // var connectionString = "Server=garden.dotvndns.vn,1444;Database=bachmoc_ver2_production;Trusted_Connection=False;User Id=bachmoc;Password=zaQ@1234";
            
            // YGFL
            var connectionString = "Server=103.114.104.24,1444;Database=factory;Trusted_Connection=False;User Id=sa;Password=zaQ@1234";
            builder.UseSqlServer(connectionString);
            return new EcommerceDbContext(builder.Options);
        }
    }
}

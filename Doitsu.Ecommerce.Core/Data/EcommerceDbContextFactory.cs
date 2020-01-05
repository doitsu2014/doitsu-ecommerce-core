using System.IO;
using Doitsu.Ecommerce.Core.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Hris.Data.Identity
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
            var connectionString = "Server=103.114.104.24,1433;Database=website.phonecard.dev.ProductService;Trusted_Connection=False;User Id=sa;Password=zaQ@1234";
            builder.UseSqlServer(connectionString);
            return new EcommerceDbContext(builder.Options);
        }
    }
}

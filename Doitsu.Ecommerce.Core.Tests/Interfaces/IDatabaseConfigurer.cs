using Microsoft.EntityFrameworkCore;

namespace Doitsu.Ecommerce.Core.Tests.Interfaces
{
    public interface IDatabaseConfigurer
    {
        void Configure(DbContextOptionsBuilder builder, string migrationAssembly);
    }
}
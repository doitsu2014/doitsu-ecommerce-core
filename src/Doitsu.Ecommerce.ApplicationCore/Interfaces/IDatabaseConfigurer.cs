using Microsoft.EntityFrameworkCore;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces
{
    public interface IDatabaseConfigurer
    {
        void Configure (DbContextOptionsBuilder builder, string migrationAssembly); 
    }
}
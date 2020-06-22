using Microsoft.EntityFrameworkCore;

namespace Doitsu.Service.Core.Interfaces
{
    public interface IDatabaseConfigurer
    {
        void Configure (DbContextOptionsBuilder builder, string migrationAssembly); 
    }
}
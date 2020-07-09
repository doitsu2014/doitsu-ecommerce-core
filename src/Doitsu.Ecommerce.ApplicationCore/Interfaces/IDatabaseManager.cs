using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces
{
    public interface IDatabaseManager
    {
        Task<IDbContextTransaction> GetDatabaseContextTransactionAsync();
    }

    public interface IEcommerceDatabaseManager : IDatabaseManager
    {
    }
}
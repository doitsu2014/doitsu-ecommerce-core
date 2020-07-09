using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace Doitsu.Ecommerce.Infrastructure.Data
{
    public class EcommerceDbManager : IEcommerceDatabaseManager 
    {
        private readonly EcommerceDbContext _dbContext;
        public EcommerceDbManager(EcommerceDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IDbContextTransaction> GetDatabaseContextTransactionAsync()
        {
            return _dbContext.Database.CurrentTransaction == null 
                ? await _dbContext.Database.BeginTransactionAsync() 
                : _dbContext.Database.CurrentTransaction;
        }
    }
}
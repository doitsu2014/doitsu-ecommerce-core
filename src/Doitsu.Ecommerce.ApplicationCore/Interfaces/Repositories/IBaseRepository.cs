using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories
{
    public interface IBaseRepository<TEntity>
        where TEntity : Entity
    {
        Task<IReadOnlyList<TEntity>> ListAllAsync();

        Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec);

        Task<TEntity> AddAsync(TEntity entity);

        Task<TEntity[]> AddRangeAsync(TEntity[] entities);

        Task UpdateAsync(TEntity entity);

        Task UpdateRangeAsync(TEntity[] entities);

        Task DeleteAsync(TEntity entity);

        Task DeleteRangeAsync(TEntity[] entities);

        Task DeleteByKeyAsync(object key);

        Task DeleteRangeByKeysAsync(object[] keys);

        Task<int> CountAsync(ISpecification<TEntity> spec);

        Task<bool> AnyAsync(ISpecification<TEntity> spec);

        Task<TEntity> FirstAsync(ISpecification<TEntity> spec);

        Task<TEntity> FirstOrDefaultAsync(ISpecification<TEntity> spec);

        Task<TEntity> FindByKeysAsync(params object[] keys);

    }
}

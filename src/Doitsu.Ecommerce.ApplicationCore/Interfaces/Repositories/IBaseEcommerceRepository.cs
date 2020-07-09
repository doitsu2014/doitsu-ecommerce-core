using System.Threading.Tasks;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories
{
    public interface IBaseEcommerceRepository<TEntity> : IBaseRepository<TEntity>
        where TEntity : Entity
    {
        Task DeleteAsync(TEntity entity, bool isForcedMode = false);

        Task DeleteRangeAsync(TEntity[] entities, bool isForcedMode = false);

        Task DeleteByKeyAsync(object key, bool isForcedMode = false);

        Task DeleteRangeByKeysAsync(object[] keys, bool isForcedMode = false);
    }
}
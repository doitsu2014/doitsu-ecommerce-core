using Doitsu.Ecommerce.ApplicationCore;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.Infrastructure.Data;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doitsu.Ecommerce.Infrastructure.Repositories
{
    public class BaseEcommerceRepository<TEntity> : BaseRepository<EcommerceDbContext, TEntity>, IBaseEcommerceRepository<TEntity>
        where TEntity : Entity
    {
        public BaseEcommerceRepository(EcommerceDbContext dbContext, ILogger<BaseRepository<EcommerceDbContext, TEntity>> logger) : base(dbContext, logger)
        {
        }

        public virtual async Task DeleteAsync(TEntity entity, bool isForcedMode = false)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            if (isForcedMode)
                await _dbContext.SaveChangesWithoutBeforeSavingAsync();
            else
                await _dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteRangeAsync(TEntity[] entities, bool isForcedMode = false)
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);

            if (isForcedMode)
                await _dbContext.SaveChangesWithoutBeforeSavingAsync();
            else
                await _dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteByKeyAsync(object key, bool isForcedMode = false)
        {
            var entity = await this.FindByKeysAsync(key);
            await this.DeleteAsync(entity);
        }

        public virtual async Task DeleteRangeByKeysAsync(object[] keys, bool isForcedMode = false)
        {
            var entities = new List<TEntity>();
            foreach (var key in keys)
            {
                var entity = await this.FindByKeysAsync(key);
                entities.Add(entity);
            }
            await this.DeleteRangeAsync(entities.ToArray());
        }
    }
}

using Doitsu.Ecommerce.ApplicationCore;
using Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;

namespace Repositories
{
    public class BaseEcommerceCachedRepository<TRepository, TEntity> : BaseCachedRepository<TRepository, TEntity>, IBaseEcommerceCachedRepository<TEntity>
        where TEntity : Entity
        where TRepository : IBaseEcommerceRepository<TEntity>
    {
        public BaseEcommerceCachedRepository(TRepository repository, ILogger<BaseCachedRepository<TRepository, TEntity>> logger, IMemoryCache memoryCache) : base(repository, logger, memoryCache)
        {
        }
    }
}
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;

namespace Interfaces.Repositories
{
    public interface IBaseCachedRepository<TEntity>
        where TEntity : Entity
    {
        Task<IReadOnlyList<TEntity>> ListAllAsync();

        Task<IReadOnlyList<TEntity>> ListAllAsync(TimeSpan cacheTime);

        Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec);

        Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> spec);
    }

    public interface IBaseEcommerceCachedRepository<TEntity> : IBaseCachedRepository<TEntity>
        where TEntity : Entity
    {
    }
}
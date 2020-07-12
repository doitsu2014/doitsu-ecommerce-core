using System.Collections.Generic;
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Interfaces.Repositories;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace Repositories
{
    public class BaseCachedRepository<TRepository, TEntity> : IBaseCachedRepository<TEntity>
        where TEntity : Entity
        where TRepository : IBaseRepository<TEntity>
    {
        protected readonly ILogger<BaseCachedRepository<TRepository, TEntity>> _logger;
        protected readonly TRepository _repository;
        protected readonly IMemoryCache _memoryCache;

        public BaseCachedRepository(TRepository repository, ILogger<BaseCachedRepository<TRepository, TEntity>> logger, IMemoryCache memoryCache)
        {
            _logger = logger;
            _repository = repository;
            _memoryCache = memoryCache;
        }

        public async Task<IReadOnlyList<TEntity>> ListAllAsync()
        {
            var cacheKey = $"{nameof(BaseCachedRepository<TRepository, TEntity>)}-{nameof(ListAllAsync)}";
            if (!_memoryCache.TryGetValue(cacheKey, out IReadOnlyList<TEntity> result))
            {
                result = await _repository.ListAllAsync();
                _memoryCache.Set(cacheKey, result);
            }
            return result;
        }

        public async Task<IReadOnlyList<TEntity>> ListAllAsync(TimeSpan cacheTime)
        {
            var cacheKey = $"{nameof(BaseCachedRepository<TRepository, TEntity>)}-{nameof(ListAllAsync)}";
            if (!_memoryCache.TryGetValue(cacheKey, out IReadOnlyList<TEntity> result))
            {
                result = await _repository.ListAllAsync();
                _memoryCache.Set(cacheKey, result, cacheTime);
            }
            return result;
        }

        public async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec)
        {
            if (!spec.CacheEnabled) throw new ArgumentException($"{nameof(spec)}.{nameof(spec.CacheEnabled)} arg is false.");

            var cacheKey = $"{nameof(BaseCachedRepository<TRepository, TEntity>)}-{nameof(ListAsync)}-{spec.CacheKey}";
            if (!_memoryCache.TryGetValue($"{cacheKey}", out IReadOnlyList<TEntity> result))
            {
                result = await _repository.ListAsync(spec);
                if(spec.CacheTime.HasValue)
                    _memoryCache.Set(cacheKey, result, spec.CacheTime.Value);
                else
                    _memoryCache.Set(cacheKey, result);
            }
            return result;
        }

        public async Task<IReadOnlyList<TResult>> ListAsync<TResult>(ISpecification<TEntity, TResult> spec)
        {
            if (!spec.CacheEnabled) throw new ArgumentException($"{nameof(spec)}.{nameof(spec.CacheEnabled)} arg is false.");

            var cacheKey = $"{nameof(BaseCachedRepository<TRepository, TEntity>)}-{nameof(ListAsync)}-{spec.CacheKey}";
            if (!_memoryCache.TryGetValue($"{cacheKey}", out IReadOnlyList<TResult> result))
            {
                result = await _repository.ListAsync<TResult>(spec);
                if(spec.CacheTime.HasValue)
                    _memoryCache.Set(cacheKey, result, spec.CacheTime.Value);
                else
                    _memoryCache.Set(cacheKey, result);
            }
            return result;
        }
    }
}
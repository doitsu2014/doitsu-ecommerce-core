using Doitsu.Ecommerce.ApplicationCore;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Doitsu.Ecommerce.Infrastructure.Repositories
{
    public abstract class BaseRepository<TDbContext, TEntity> : IBaseRepository<TEntity>
        where TDbContext : DbContext
        where TEntity : Entity
    {
        protected readonly TDbContext _dbContext;
        protected readonly ILogger<BaseRepository<TDbContext, TEntity>> _logger;

        public BaseRepository(TDbContext dbContext, ILogger<BaseRepository<TDbContext, TEntity>> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public virtual async Task<IReadOnlyList<TEntity>> ListAllAsync()
        {
            _logger.LogDebug("{functionName}", nameof(ListAsync));
            return await _dbContext.Set<TEntity>().ToListAsync();
        }

        public virtual async Task<IReadOnlyList<TEntity>> ListAsync(ISpecification<TEntity> spec)
        {
            _logger.LogDebug("{functionName} {spec}", nameof(ListAsync), spec);
            var specificationResult = ApplySpecification(spec);
            return await specificationResult.ToListAsync();
        }

        public virtual async Task<int> CountAsync(ISpecification<TEntity> spec)
        {
            _logger.LogDebug("{functionName} {spec}", nameof(CountAsync), spec);
            var specificationResult = ApplySpecification(spec);
            return await specificationResult.CountAsync();
        }

        public virtual async Task<TEntity> AddAsync(TEntity entity)
        {
            _logger.LogDebug("{functionName} {spec}", nameof(AddAsync), entity);
            await _dbContext.Set<TEntity>().AddAsync(entity);
            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            _logger.LogDebug("{functionName} {spec}", nameof(UpdateAsync), entity);
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task<TEntity> FirstAsync(ISpecification<TEntity> spec)
        {
            var specificationResult = ApplySpecification(spec);
            return await specificationResult.FirstAsync();
        }

        public virtual async Task<TEntity> FirstOrDefaultAsync(ISpecification<TEntity> spec)
        {
            var specificationResult = ApplySpecification(spec);
            return await specificationResult.FirstOrDefaultAsync();
        }

        public virtual async Task<bool> AnyAsync(ISpecification<TEntity> spec)
        {
            var specificationResult = ApplySpecification(spec);
            return await specificationResult.AnyAsync();
        }

        public virtual async Task<TEntity> FindByKeysAsync(params object[] keys)
        {
            return await _dbContext.Set<TEntity>().FindAsync(keys);
        }

        private IQueryable<TEntity> ApplySpecification(ISpecification<TEntity> spec)
        {
            return EfSpecificationEvaluator<TEntity>.GetQuery(_dbContext.Set<TEntity>().AsQueryable(), spec);
        }

        public virtual async Task<TEntity[]> AddRangeAsync(TEntity[] entities)
        {
            await _dbContext.Set<TEntity>().AddRangeAsync(entities);
            await _dbContext.SaveChangesAsync();
            return entities;
        }

        public virtual async Task UpdateRangeAsync(TEntity[] entities)
        {
            foreach (var entity in entities)
            {
                _dbContext.Entry(entity).State = EntityState.Modified;
            }
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            _dbContext.Set<TEntity>().Remove(entity);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteRangeAsync(TEntity[] entities)
        {
            _dbContext.Set<TEntity>().RemoveRange(entities);
            await _dbContext.SaveChangesAsync();
        }

        public virtual async Task DeleteByKeyAsync(object key)
        {
            var entity = await this.FindByKeysAsync(key);
            await this.DeleteAsync(entity);
        }

        public virtual async Task DeleteRangeByKeysAsync(object[] keys)
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
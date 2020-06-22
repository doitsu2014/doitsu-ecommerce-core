using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Doitsu.Service.Core.Services.DataService
{
    public abstract class BaseService<TEntity, TDbContext> : IBaseService<TEntity, TDbContext>
        where TEntity : class, new()
    where TDbContext : DbContext
    {
        public TDbContext DbContext { get; }
        public IMapper Mapper { get; }
        public DbSet<TEntity> SelfRepository { get; }
        public ILogger<BaseService<TEntity, TDbContext>> Logger { get; }
        private IDbContextTransaction CurrentTransaction { get; set; }

        public BaseService(TDbContext dbContext, IMapper mapper, ILogger<BaseService<TEntity, TDbContext>> logger)
        {
            this.Logger = logger;
            this.DbContext = dbContext;
            this.SelfRepository = dbContext.Set<TEntity>();
            this.Mapper = mapper;
        }

        #region Access Service

        public async Task<TViewModel> FindByKeysAsync<TViewModel>(params object[] keys)
        {
            var entity = await FindByKeysAsync(keys);
            return Mapper.Map<TViewModel>(entity);
        }

        public async Task<TViewModel> FindByKeysWithoutTrackingAsync<TViewModel>(params object[] keys)
        {
            var entity = await FindByKeysWithoutTrackingAsync(keys);
            return Mapper.Map<TViewModel>(entity);
        }

        public virtual TEntity FindById(int id)
        {
            var entity = SelfRepository.Find(id);
            return entity;
        }

        public virtual async Task<TEntity> FindByIdAsync(int id)
        {
            var entity = await SelfRepository.FindAsync(id);
            return entity;
        }

        public virtual TEntity FindById<TKey>(TKey id)
        {
            var entity = SelfRepository.Find(id);
            return entity;
        }

        public virtual async Task<TEntity> FindByIdAsync<TKey>(TKey id)
        {
            var entity = await SelfRepository.FindAsync(id);
            return entity;
        }

        public virtual async Task<TEntity> FindByKeysAsync(params object[] keys)
        {
            var entity = await SelfRepository.FindAsync(keys);
            return entity;
        }

        public virtual async Task<TEntity> FindByKeysWithoutTrackingAsync(params object[] keys)
        {
            var entity = await SelfRepository.FindAsync(keys);
            DbContext.Entry<TEntity>(entity).State = EntityState.Detached;
            return entity;
        }

        public virtual IQueryable<TEntity> GetAll(bool isTracking = false)
        {
            return isTracking switch
            {
                false => GetAllAsNoTracking(),
                _ => GetAllAsTracking()
            };
        }


        public IQueryable<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false)
        {
            return isTracking switch
            {
                false => GetAllAsNoTracking(handler),
                _ => GetAllAsTracking(handler)
            };
        }

        public virtual IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, bool isTracking = false)
        {
            return isTracking switch
            {
                false => GetAsNoTracking(predicate),
                _ => GetAsTracking(predicate)
            };
        }

        public IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false)
        {
            return isTracking switch
            {
                false => GetAsNoTracking(predicate, handler),
                _ => GetAsTracking(predicate, handler)
            };
        }

        public virtual IQueryable<TViewModel> GetAll<TViewModel>(bool isTracking = false)
        {
            return isTracking switch
            {
                false => GetAllAsNoTracking().ProjectTo<TViewModel>(Mapper.ConfigurationProvider),
                _ => GetAllAsTracking().ProjectTo<TViewModel>(Mapper.ConfigurationProvider)
            };
        }

        public virtual IQueryable<TViewModel> GetAll<TViewModel>(Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false)
        {
            return isTracking switch
            {
                false => GetAllAsNoTracking(handler).ProjectTo<TViewModel>(Mapper.ConfigurationProvider),
                _ => GetAllAsTracking(handler).ProjectTo<TViewModel>(Mapper.ConfigurationProvider)
            };
        }

        public virtual IQueryable<TViewModel> Get<TViewModel>(Expression<Func<TEntity, bool>> predicate, bool isTracking = false)
        {
            return isTracking switch
            {
                false => GetAsNoTracking(predicate).ProjectTo<TViewModel>(Mapper.ConfigurationProvider),
                _ => GetAsTracking(predicate).ProjectTo<TViewModel>(Mapper.ConfigurationProvider)
            };
        }

        public virtual IQueryable<TViewModel> Get<TViewModel>(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false)
        {
            return isTracking switch
            {
                false => GetAsNoTracking(predicate, handler).ProjectTo<TViewModel>(Mapper.ConfigurationProvider),
                _ => GetAsTracking(predicate, handler).ProjectTo<TViewModel>(Mapper.ConfigurationProvider)
            };
        }

        public virtual TEntity FirstOrDefault(bool isTracking = false)
        {
            return isTracking switch
            {
                false => GetAllAsNoTracking().FirstOrDefault(),
                _ => GetAllAsTracking().FirstOrDefault()
            };
        }

        public virtual TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, bool isTracking = false)
        {
            return isTracking switch
            {
                false => GetAsNoTracking(predicate).FirstOrDefault(),
                _ => GetAsTracking(predicate).FirstOrDefault()
            };
        }

        public virtual async Task<TEntity> FirstOrDefaultAsync(bool isTracking = false)
        {
            return isTracking switch
            {
                false => await GetAllAsNoTracking().FirstOrDefaultAsync(),
                _ => await GetAllAsTracking().FirstOrDefaultAsync()
            };
        }

        public virtual async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool isTracking = false)
        {
            return isTracking switch
            {
                false => await GetAsNoTracking(predicate).FirstOrDefaultAsync(),
                _ => await GetAsTracking(predicate).FirstOrDefaultAsync()
            };
        }

        public virtual async Task<TViewModel> FirstOrDefaultAsync<TViewModel>(Expression<Func<TEntity, bool>> predicate, bool isTracking = false)
        {
            return isTracking switch
            {
                false => await GetAsNoTracking(predicate).ProjectTo<TViewModel>(Mapper.ConfigurationProvider).FirstOrDefaultAsync(),
                _ => await GetAsTracking(predicate).ProjectTo<TViewModel>(Mapper.ConfigurationProvider).FirstOrDefaultAsync()
            };
        }

        public async Task<TViewModel> FirstOrDefaultAsync<TViewModel>(bool isTracking = false)
        {
            var entity = await FirstOrDefaultAsync(isTracking);
            return Mapper.Map<TViewModel>(entity);
        }

        public TEntity FirstOrDefault(Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false)
        {
            return isTracking switch
            {
                false => GetAllAsNoTracking(handler).FirstOrDefault(),
                _ => GetAllAsTracking(handler).FirstOrDefault()
            };
        }

        public TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false)
        {
            return isTracking switch
            {
                false => GetAsNoTracking(predicate, handler).FirstOrDefault(),
                _ => GetAsTracking(predicate, handler).FirstOrDefault()
            };
        }

        public async Task<TEntity> FirstOrDefaultAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false)
        {
            return isTracking switch
            {
                false => await GetAllAsNoTracking(handler).FirstOrDefaultAsync(),
                _ => await GetAllAsTracking(handler).FirstOrDefaultAsync()
            };

        }
        public async Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false)
        {
            return isTracking switch
            {
                false => await GetAsNoTracking(predicate, handler).FirstOrDefaultAsync(),
                _ => await GetAsTracking(predicate, handler).FirstOrDefaultAsync()
            };
        }

        public async Task<TViewModel> FirstOrDefaultAsync<TViewModel>(Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false)
        {
            var entity = await FirstOrDefaultAsync(handler, isTracking);
            return Mapper.Map<TViewModel>(entity);
        }


        public async Task<TViewModel> FirstOrDefaultAsync<TViewModel>(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false)
        {
            var entity = await FirstOrDefaultAsync(predicate, handler, isTracking);
            return Mapper.Map<TViewModel>(entity);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return SelfRepository.AnyAsync(predicate);
        }

        public Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate)
        {
            return SelfRepository.AllAsync(predicate);
        }

        #endregion

        #region Business Service

        public virtual TEntity Create(TEntity entity)
        {
            SelfRepository.Add(entity);
            return entity;
        }

        public virtual async Task<TEntity> CreateAsync(TEntity entity)
        {
            await SelfRepository.AddAsync(entity);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> CreateAsync(IEnumerable<TEntity> entities)
        {
            await SelfRepository.AddRangeAsync(entities);
            return entities;
        }

        public virtual TEntity Create<TViewModel>(TViewModel viewModel)
        {
            var entity = Mapper.Map<TEntity>(viewModel);
            return Create(entity);
        }

        public virtual async Task<TEntity> CreateAsync<TViewModel>(TViewModel viewModel)
        {
            var entity = Mapper.Map<TEntity>(viewModel);
            return await CreateAsync(entity);
        }

        public async Task<IEnumerable<TEntity>> CreateAsync<TViewModel>(IEnumerable<TViewModel> viewModels)
        {
            var entities = viewModels.Select(x => Mapper.Map<TEntity>(x)).AsEnumerable();
            return await CreateAsync(entities);
        }

        public async Task<TKey> DeleteAsync<TKey>(TKey id)
        {
            var entity = await SelfRepository.FindAsync(id);
            SelfRepository.Remove(entity);
            return id;
        }

        public virtual TEntity Update(TEntity entity)
        {
            SelfRepository.Update(entity);
            return entity;
        }

        public virtual ICollection<TEntity> UpdateRange(ICollection<TEntity> entities)
        {
            SelfRepository.UpdateRange(entities);
            return entities;
        }

        public virtual TEntity Update<TViewModel>(TViewModel viewModel)
        {
            var entity = Mapper.Map<TEntity>(viewModel);
            SelfRepository.Update(entity);
            return entity;
        }

        public virtual IEnumerable<TEntity> UpdateRange<TViewModel>(ICollection<TViewModel> viewModels)
        {
            var entities = viewModels.Select(vm => Mapper.Map<TEntity>(vm)).ToImmutableList();
            UpdateRange(entities);
            return entities;
        }

        public async Task<ICollection<TKey>> DeleteAsync<TKey>(ICollection<TKey> ids)
        {
            foreach (var id in ids)
            {
                var entity = await SelfRepository.FindAsync(id);
                SelfRepository.Remove(entity);
            }
            return ids;
        }

        #endregion

        #region Integration Service

        public virtual Task<int> CommitAsync(bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = default)
        {
            return DbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public virtual int Commit()
        {
            return DbContext.SaveChanges();
        }

        public virtual async Task<IDbContextTransaction> CreateTransactionAsync()
        {
            CurrentTransaction = await DbContext.Database.BeginTransactionAsync();
            return CurrentTransaction;
        }

        public virtual IDbContextTransaction CreateTransaction()
        {
            CurrentTransaction = DbContext.Database.BeginTransaction();
            return CurrentTransaction;
        }

        public void DisposeDbContext()
        {
            DbContext.Dispose();
        }

        public async Task DisposeDbContextAsync()
        {
            await DbContext.DisposeAsync();
        }

        public EntityEntry Attach(TEntity ent)
        {
            return SelfRepository.Attach(ent);
        }

        public void AttachRange(TEntity[] entities)
        {
            SelfRepository.AttachRange(entities);
        }

        public EntityEntry Attach<TViewModel>(TViewModel vm)
        {
            var attachedEnt = Mapper.Map<TEntity>(vm);
            return this.Attach(attachedEnt);
        }

        public void AttachRange<TViewModel>(TViewModel[] vms)
        {
            var attachedEnts = vms.Select(vm => Mapper.Map<TEntity>(vm)).ToArray();
            this.AttachRange(attachedEnts);
        }

        public void MarkModifiedOrAdded(TEntity ent)
        {
            var entry = DbContext.Entry(ent);
            if (!entry.IsKeySet)
            {
                entry.State = EntityState.Added;
            }
            else
            {
                entry.State = EntityState.Modified;
            }
        }

        public void MarkModifiedOrAddedWithTrackGraph(TEntity ent)
        {
            DbContext.ChangeTracker.TrackGraph(ent, e =>
            {
                e.Entry.State = EntityState.Unchanged;
                if (!e.Entry.IsKeySet)
                {
                    e.Entry.State = EntityState.Added;
                }
                else
                {
                    e.Entry.State = EntityState.Modified;
                }
            });
        }

        #endregion

        #region Utils

        protected IQueryable<TEntity> GetAllAsNoTracking()
        {
            return SelfRepository.AsNoTracking();
        }

        protected IQueryable<TEntity> GetAllAsNoTracking(Func<IQueryable<TEntity>, IQueryable<TEntity>> handler)
        {
            var query = SelfRepository.AsQueryable();
            query = handler.Invoke(query);
            query = query.AsNoTracking();
            return query;
        }

        protected IQueryable<TEntity> GetAsNoTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return SelfRepository.AsNoTracking().Where(predicate);
        }

        protected IQueryable<TEntity> GetAsNoTracking(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> handler)
        {
            var query = SelfRepository.AsQueryable();
            query = handler.Invoke(query);
            query = query.Where(predicate).AsNoTracking();
            return query;
        }

        protected IQueryable<TEntity> GetAllAsTracking()
        {
            return SelfRepository.AsTracking();
        }

        protected IQueryable<TEntity> GetAllAsTracking(Func<IQueryable<TEntity>, IQueryable<TEntity>> handler)
        {
            var query = SelfRepository.AsQueryable();
            query = handler.Invoke(query);
            query = query.AsTracking();
            return query;
        }

        protected IQueryable<TEntity> GetAsTracking(Expression<Func<TEntity, bool>> predicate)
        {
            return SelfRepository.AsTracking().Where(predicate);
        }

        protected IQueryable<TEntity> GetAsTracking(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> handler)
        {
            var query = SelfRepository.AsQueryable();
            query = handler.Invoke(query);
            query = query
                .Where(predicate)
                .AsTracking();
            return query;
        }

        protected DbSet<TDbSet> GetRepository<TDbSet>()
        where TDbSet : class
        {
            return DbContext.Set<TDbSet>();
        }

        #endregion
    }

}

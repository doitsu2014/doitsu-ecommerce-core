using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage;

namespace Doitsu.Service.Core
{
    public interface IBaseService<TEntity, TDbContext>
        where TEntity : class, new()
        where TDbContext : DbContext
    {
        #region Access Service
        /// <summary>
        /// Finds the by identifier.
        /// But identitifier default is integer
        /// </summary>
        /// <returns>The by identifier.</returns>
        /// <param name="id">Identifier.</param>
        TEntity FindById(int id);
        
        /// <summary>
        /// Finds the by identifier async.
        /// But identitifier default is integer
        /// </summary>
        /// <returns>The by identifier async.</returns>
        /// <param name="id">Identifier.</param>
        Task<TEntity> FindByIdAsync(int id);
        
        /// <summary>
        /// Finds the by identifier.
        /// </summary>
        /// <returns>The by identifier.</returns>
        /// <param name="id">Identifier.</param>
        /// <typeparam name="TKey">The 1st type parameter.</typeparam>
        TEntity FindById<TKey>(TKey id);
        
        Task<TEntity> FindByIdAsync<TKey>(TKey id);
        
        /// <summary>
        /// Find entity with keys as tracking
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<TEntity> FindByKeysAsync(params object[] keys);
        
        /// <summary>
        /// Find entity with keys, and make state is detached
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<TEntity> FindByKeysWithoutTrackingAsync(params object[] keys);

        /// <summary>
        /// Find viewmodel with keys as tracking
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<TViewModel> FindByKeysAsync<TViewModel>(params object[] keys);
        
        /// <summary>
        /// Find viewmodel with keys, and make state is detached
        /// </summary>
        /// <param name="keys"></param>
        /// <returns></returns>
        Task<TViewModel> FindByKeysWithoutTrackingAsync<TViewModel>(params object[] keys);

        /// <summary>
        /// Gets all, get all no tracking data.
        /// </summary>
        /// <returns>The all.</returns>
        IQueryable<TEntity> GetAll(bool isTracking = false);
        
        /// <summary>
        /// Get data with filter expression
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="predicate">Predicate.</param>
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, bool isTracking = false);
        
        /// <summary>
        /// Gets all, get all no tracking data.
        /// </summary>
        /// <returns>The all.</returns>
        IQueryable<TViewModel> GetAll<TViewModel>(bool isTracking = false);

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="handler"></param>
        /// <param name="isTracking"></param>
        /// <returns></returns>
        IQueryable<TViewModel> GetAll<TViewModel>(Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false);
        
        /// <summary>
        /// Get data with filter expression
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="predicate">Predicate.</param>
        IQueryable<TViewModel> Get<TViewModel>(Expression<Func<TEntity, bool>> predicate, bool isTracking = false);

        /// <summary>
        /// Get data with filter expression
        /// </summary>
        /// <typeparam name="TViewModel"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="handler"></param>
        /// <param name="isTracking"></param>
        /// <returns></returns>
        IQueryable<TViewModel> Get<TViewModel>(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false);

        /// <summary>
        /// Get all data with handler
        /// </summary>
        /// <param name="handler">use for Include, ThenInclude,..</param>
        /// <param name="isTracking"></param>
        /// <returns></returns>
        IQueryable<TEntity> GetAll(Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false);

        /// <summary>
        /// Get data with expression and handler
        /// </summary>
        /// <returns>The get.</returns>
        /// <param name="predicate">Predicate.</param>
        /// <param name="handler">use for Include, ThenInclude,..</param>
        /// <param name="isTracking"></param>
        IQueryable<TEntity> Get(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false);

        /// <summary>
        /// Firsts the or default. As no tracking
        /// </summary>
        /// <returns>The or default.</returns>
        TEntity FirstOrDefault(bool isTracking = false);

        TEntity FirstOrDefault(Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, bool isTracking = false);

        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false);
        
        Task<TEntity> FirstOrDefaultAsync(bool isTracking = false);

        Task<TEntity> FirstOrDefaultAsync(Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false);

        Task<TViewModel> FirstOrDefaultAsync<TViewModel>(bool isTracking = false);

        Task<TViewModel> FirstOrDefaultAsync<TViewModel>(Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, bool isTracking = false);

        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false);

        Task<TViewModel> FirstOrDefaultAsync<TViewModel>(Expression<Func<TEntity, bool>> predicate, bool isTracking = false);

        Task<TViewModel> FirstOrDefaultAsync<TViewModel>(Expression<Func<TEntity, bool>> predicate, Func<IQueryable<TEntity>, IQueryable<TEntity>> handler, bool isTracking = false);

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate);
        
        Task<bool> AllAsync(Expression<Func<TEntity, bool>> predicate);
        
        #endregion
        #region Business Service
        
        TEntity Create(TEntity entity);
        
        Task<TEntity> CreateAsync(TEntity entity);
        
        Task<IEnumerable<TEntity>> CreateAsync(IEnumerable<TEntity> entities);
        
        TEntity Create<TViewModel>(TViewModel viewModel);
        
        Task<TEntity> CreateAsync<TViewModel>(TViewModel viewModel);
        
        Task<IEnumerable<TEntity>> CreateAsync<TViewModel>(IEnumerable<TViewModel> viewModels);

        TEntity Update(TEntity entity);
        
        ICollection<TEntity> UpdateRange(ICollection<TEntity> entities);
        
        TEntity Update<TViewModel>(TViewModel viewModel);
        
        IEnumerable<TEntity> UpdateRange<TViewModel>(ICollection<TViewModel> viewModels);

        Task<TKey> DeleteAsync<TKey>(TKey id);
        
        Task<ICollection<TKey>> DeleteAsync<TKey>(ICollection<TKey> ids);
        #endregion
        #region Integration Service
        
        Task<int> CommitAsync(bool acceptAllChangesOnSuccess = true, CancellationToken cancellationToken = new CancellationToken());
        
        int Commit();
        
        Task<IDbContextTransaction> CreateTransactionAsync();
        
        IDbContextTransaction CreateTransaction();
        
        void DisposeDbContext();
        
        Task DisposeDbContextAsync();
        
        EntityEntry Attach(TEntity ent);
        
        void AttachRange(TEntity[] entities);
        
        EntityEntry Attach<TViewModel>(TViewModel vm);
        
        void AttachRange<TViewModel>(TViewModel[] vms);
        
        TDbContext DbContext { get; }

        IMapper Mapper { get; }

        DbSet<TEntity> SelfRepository { get; }

        void MarkModifiedOrAddedWithTrackGraph(TEntity ent);

        void MarkModifiedOrAdded(TEntity ent) ;

        #endregion
    }

    

}
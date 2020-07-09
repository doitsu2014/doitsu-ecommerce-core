using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.ViewModelServices
{
    public interface IBaseViewModelService<TRepository, TEntity>
        where TRepository : IBaseRepository<TEntity>
        where TEntity : Entity
    {
        Task<IReadOnlyList<TViewModel>> ListAllAsync<TViewModel>();

        Task<IReadOnlyList<TViewModel>> ListAsync<TViewModel>(ISpecification<TEntity> spec);

        Task<TViewModel> AddAsync<TViewModel>(TViewModel data);

        Task<TViewModel[]> AddRangeAsync<TViewModel>(TViewModel[] listData);

        Task UpdateAsync<TViewModel>(TViewModel data);

        Task UpdateRangeAsync<TViewModel>(TViewModel[] listData);

        Task DeleteAsync<TViewModel>(TViewModel data);

        Task DeleteRangeAsync<TViewModel>(TViewModel[] listData);

        Task DeleteByKeyAsync(object key);

        Task DeleteRangeByKeysAsync(object[] keys);

        Task<int> CountAsync(ISpecification<TEntity> spec);

        Task<bool> AnyAsync(ISpecification<TEntity> spec);

        Task<TViewModel> FirstAsync<TViewModel>(ISpecification<TEntity> spec);

        Task<TViewModel> FirstOrDefaultAsync<TViewModel>(ISpecification<TEntity> spec);

        Task<TViewModel> FindByKeysAsync<TViewModel>(params object[] keys);
    }
}
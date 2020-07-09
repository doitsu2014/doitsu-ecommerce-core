using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using System.Threading.Tasks;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.ViewModelServices
{
    public interface IBaseEcommerceViewModelService<TEntity> : IBaseViewModelService<IBaseEcommerceRepository<TEntity>, TEntity>
        where TEntity : Entity
    {
        Task DeleteAsync<TViewModel>(TViewModel data, bool isForcedMode = false);

        Task DeleteRangeAsync<TViewModel>(TViewModel[] listData, bool isForcedMode = false);

        Task DeleteByKeyAsync(object key, bool isForcedMode = false);

        Task DeleteRangeByKeysAsync(object[] keys, bool isForcedMode = false);
        
    }
}
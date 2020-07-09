using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.ViewModelServices;

namespace Doitsu.Ecommerce.ApplicationCore.Services.ViewModelServices
{
    public class BaseEcommerceViewModelService<TEntity> : BaseViewModelService<IBaseEcommerceRepository<TEntity>, TEntity>, IBaseEcommerceViewModelService<TEntity> 
        where TEntity : Entity
    {
        public BaseEcommerceViewModelService(IBaseEcommerceRepository<TEntity> mainRepository, IMapper mapper) : base(mainRepository, mapper)
        {
        }

        public virtual async Task DeleteAsync<TViewModel>(TViewModel data, bool isForcedMode = false)
        {
            var entity = mapper.Map<TEntity>(data);
            await mainRepository.DeleteAsync(entity, isForcedMode);
        }

        public virtual async Task DeleteByKeyAsync(object key, bool isForcedMode = false)
        {
            await mainRepository.DeleteByKeyAsync(key, isForcedMode);
        }

        public virtual async Task DeleteRangeAsync<TViewModel>(TViewModel[] listData, bool isForcedMode = false)
        {
            var listEntities = listData.Select(d => mapper.Map<TEntity>(d)).ToArray();
            await mainRepository.DeleteRangeAsync(listEntities, isForcedMode);
        }

        public virtual async Task DeleteRangeByKeysAsync(object[] keys, bool isForcedMode = false)
        {
            await mainRepository.DeleteRangeByKeysAsync(keys, isForcedMode);
        }

    }
}
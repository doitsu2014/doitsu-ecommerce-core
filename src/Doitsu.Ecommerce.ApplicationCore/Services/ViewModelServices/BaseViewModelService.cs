using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using AutoMapper;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Repositories;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.ViewModelServices;

namespace Doitsu.Ecommerce.ApplicationCore.Services.ViewModelServices
{
    public abstract class BaseViewModelService<TRepository, TEntity> : IBaseViewModelService<TRepository, TEntity>
        where TRepository : IBaseRepository<TEntity>
        where TEntity : Entity
    {
        protected readonly IMapper mapper;
        protected readonly TRepository mainRepository;

        public BaseViewModelService(TRepository mainRepository, IMapper mapper)
        {
            this.mainRepository = mainRepository;
            this.mapper = mapper;
        }

        public virtual async Task<TViewModel> AddAsync<TViewModel>(TViewModel data)
        {
            var entity = mapper.Map<TEntity>(data);
            entity = await mainRepository.AddAsync(entity);
            return mapper.Map<TViewModel>(entity);
        }

        public virtual async Task<TViewModel[]> AddRangeAsync<TViewModel>(TViewModel[] listData)
        {
            var listEntities = listData.Select(d => mapper.Map<TEntity>(d)).ToArray();
            listEntities = await mainRepository.AddRangeAsync(listEntities);
            return listEntities.Select(e => mapper.Map<TViewModel>(e)).ToArray();
        }

        public virtual async Task<bool> AnyAsync(ISpecification<TEntity> spec)
        {
            return await mainRepository.AnyAsync(spec);
        }

        public virtual async Task<int> CountAsync(ISpecification<TEntity> spec)
        {
            return await mainRepository.CountAsync(spec);
        }

        public virtual async Task DeleteAsync<TViewModel>(TViewModel data)
        {
            var entity = mapper.Map<TEntity>(data);
            await mainRepository.DeleteAsync(entity);
        }

        public virtual async Task DeleteByKeyAsync(object key)
        {
            await mainRepository.DeleteByKeyAsync(key);
        }

        public virtual async Task DeleteRangeAsync<TViewModel>(TViewModel[] listData)
        {
            var listEntities = listData.Select(d => mapper.Map<TEntity>(d)).ToArray();
            await mainRepository.DeleteRangeAsync(listEntities);
        }

        public virtual async Task DeleteRangeByKeysAsync(object[] keys)
        {
            await mainRepository.DeleteRangeByKeysAsync(keys);
        }

        public virtual async Task<TViewModel> FindByKeysAsync<TViewModel>(params object[] keys)
        {
            return mapper.Map<TViewModel>(await mainRepository.FindByKeysAsync(keys));
        }

        public virtual async Task<TViewModel> FirstAsync<TViewModel>(ISpecification<TEntity> spec)
        {
            return mapper.Map<TViewModel>(await mainRepository.FirstAsync(spec));
        }

        public virtual async Task<TViewModel> FirstOrDefaultAsync<TViewModel>(ISpecification<TEntity> spec)
        {
            return mapper.Map<TViewModel>(await mainRepository.FirstOrDefaultAsync(spec));
        }

        public virtual async Task<IReadOnlyList<TViewModel>> ListAllAsync<TViewModel>()
        {
            return (await mainRepository.ListAllAsync()).Select(e => mapper.Map<TViewModel>(e)).ToList();
        }

        public virtual async Task<IReadOnlyList<TViewModel>> ListAsync<TViewModel>(ISpecification<TEntity> spec)
        {
            return (await mainRepository.ListAsync(spec)).Select(e => mapper.Map<TViewModel>(e)).ToList();
        }

        public virtual async Task UpdateAsync<TViewModel>(TViewModel data)
        {
            var entity = mapper.Map<TEntity>(data);
            await mainRepository.UpdateAsync(entity);
        }

        public virtual async Task UpdateRangeAsync<TViewModel>(TViewModel[] listData)
        {
            var listEntities = listData.Select(d => mapper.Map<TEntity>(d)).ToArray();
            await mainRepository.UpdateRangeAsync(listEntities);
        }
    }
}

using Doitsu.Ecommerce.Core.Data;
using Doitsu.Service.Core;

namespace Doitsu.Ecommerce.Core.Services.Interface
{
    public interface IBaseService<TEntity> : IBaseService<TEntity, EcommerceDbContext>
        where TEntity : class, new()
    {
    }
}

using Doitsu.Ecommerce.Core.Data;
using Doitsu.Service.Core;

namespace Doitsu.Ecommerce.Core.Abstraction.Interfaces
{
    public interface IBaseService<TEntity> : IBaseService<TEntity, EcommerceDbContext>
        where TEntity : class, new()
    {
    }
}

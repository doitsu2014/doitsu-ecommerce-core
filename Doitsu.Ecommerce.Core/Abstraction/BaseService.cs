using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;

namespace Doitsu.Ecommerce.Core.Abstraction
{
    public class BaseService<TEntity> : BaseService<TEntity, EcommerceDbContext>, IBaseService<TEntity>
        where TEntity : class, new()
    {
        public BaseService(IUnitOfWork<EcommerceDbContext> unitOfWork, ILogger<BaseService<TEntity, EcommerceDbContext>> logger) : base(unitOfWork, logger)
        {
        }
    }
}

using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;
using Doitsu.Service.Core.Services.DataService;
using Microsoft.Extensions.Logging;

namespace Doitsu.Ecommerce.Core
{

    public abstract class EcommerceBaseService<TEntity> : BaseService<TEntity, EcommerceDbContext>, IEcommerceBaseService<TEntity>
        where TEntity : class, new()
    {
        protected EcommerceBaseService(EcommerceDbContext dbContext, IMapper mapper, ILogger<EcommerceBaseService<TEntity>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
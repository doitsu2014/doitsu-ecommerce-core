using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;
using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;

namespace Doitsu.Ecommerce.Core.Services
{
    public class BaseService<TEntity> : BaseService<TEntity, EcommerceDbContext>, IBaseService<TEntity>
        where TEntity : class, new()
    {
        public BaseService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<TEntity, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }

        public override async Task<int> CommitAsync(bool acceptAllChangesOnSuccess = default, CancellationToken cancellationToken = default)
        {
            return await this.DbContext.SaveChangesWithBeforeSavingAsync();
        }

        public async Task<int> CommitWithoutBeforeSavingAsync()
        {
            return await this.DbContext.SaveChangesAsync();
        }
    }
}

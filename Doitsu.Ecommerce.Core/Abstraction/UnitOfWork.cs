using System;
using System.Threading.Tasks;
using AutoMapper;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Service.Core;

namespace Doitsu.Ecommerce.Core.Abstraction
{
    public class EcommerceUnitOfWork : UnitOfWork<EcommerceDbContext>, IEcommerceUnitOfWork
    {
        public EcommerceUnitOfWork(EcommerceDbContext dbContext, IMapper mapper, IServiceProvider serviceProvider) : base(dbContext, mapper, serviceProvider)
        {
        }

        public override async Task<int> CommitAsync()
        {
            return await this.DbContext.SaveChangesWithBeforeSavingAsync();
        }

        public async Task<int> CommitWithoutBeforeSavingAsync()
        {
            return await this.DbContext.SaveChangesAsync();
        }
    }
}

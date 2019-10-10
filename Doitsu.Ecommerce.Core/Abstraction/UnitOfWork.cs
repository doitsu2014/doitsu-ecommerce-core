using System;
using AutoMapper;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Service.Core;

namespace Doitsu.Ecommerce.Core.Abstraction
{
    public class UnitOfWork : UnitOfWork<EcommerceDbContext>, IUnitOfWork
    {
        public UnitOfWork(EcommerceDbContext dbContext, IMapper mapper, IServiceProvider serviceProvider) : base(dbContext, mapper, serviceProvider)
        {
        }
    }
}

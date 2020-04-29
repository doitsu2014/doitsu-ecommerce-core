using AutoMapper;
using Doitsu.Ecommerce.Core.Abstraction;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;

namespace Doitsu.Ecommerce.Core.Services
{
    public class WareHouseService : BaseService<WareHouse>, IBaseService<WareHouse>
    {
        public WareHouseService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<WareHouse, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
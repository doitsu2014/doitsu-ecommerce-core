using AutoMapper;
using Doitsu.Ecommerce.Core.Abstraction;

using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public class WareHouseService : EcommerceBaseService<WareHouse>, IEcommerceBaseService<WareHouse>
    {
        public WareHouseService(EcommerceDbContext dbContext, IMapper mapper, ILogger<EcommerceBaseService<WareHouse>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}
using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IOrderItemService : IBaseService<OrderItems>
    {
    }

    public class OrderItemService : BaseService<OrderItems>, IOrderItemService
    {
        public OrderItemService(EcommerceDbContext dbContext, IMapper mapper, ILogger<BaseService<OrderItems, EcommerceDbContext>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}

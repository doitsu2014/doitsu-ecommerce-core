using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;

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

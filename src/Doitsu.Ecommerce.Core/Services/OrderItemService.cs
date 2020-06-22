using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data;
using Doitsu.Ecommerce.Core.Services.Interface;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IOrderItemService : IEcommerceBaseService<OrderItems>
    {
    }

    public class OrderItemService : EcommerceBaseService<OrderItems>, IOrderItemService
    {
        public OrderItemService(EcommerceDbContext dbContext, IMapper mapper, ILogger<EcommerceBaseService<OrderItems>> logger) : base(dbContext, mapper, logger)
        {
        }
    }
}

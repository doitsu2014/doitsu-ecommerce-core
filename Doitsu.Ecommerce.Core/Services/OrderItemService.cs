using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Ecommerce.Core.Abstraction.Interfaces;
using Doitsu.Ecommerce.Core.Abstraction;
namespace Doitsu.Ecommerce.Core.Services
{
    public interface IOrderItemService : IBaseService<OrderItems>
    {
    }

    public class OrderItemService : BaseService<OrderItems>, IOrderItemService
    {
        public OrderItemService(IEcommerceUnitOfWork unitOfWork, ILogger<BaseService<OrderItems>> logger) : base(unitOfWork, logger)
        {

        }
    }
}

using Doitsu.Service.Core;
using Microsoft.Extensions.Logging;
using Doitsu.Ecommerce.Core.Data.Entities;

namespace Doitsu.Ecommerce.Core.Services
{
    public interface IOrderItemService : IBaseService<OrderItems>
    {
    }

    public class OrderItemService : BaseService<OrderItems>, IOrderItemService
    {
        public OrderItemService(IUnitOfWork unitOfWork, ILogger<BaseService<OrderItems>> logger) : base(unitOfWork, logger)
        {

        }
    }
}

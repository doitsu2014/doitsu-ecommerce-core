using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Optional;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.Services.Coordinators
{
    public interface IOrderStatusCoordinator
    {
        Task<Option<Orders, string>> ChangeOrderStatus(string orderCode, OrderStatusEnum statusEnum, int auditUserId, string note = "", bool isWorkingInventoryQuantity = false); 
    }
}
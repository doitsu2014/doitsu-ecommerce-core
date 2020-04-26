using System.Threading.Tasks;
using Doitsu.Ecommerce.DeliveryIntegration.Common;
using Optional;

namespace Doitsu.Ecommerce.DeliveryIntegration.Interfaces
{
    public interface IGHTKService
    {
        Task<Option<dynamic, string>> CalculateFees(CalculateDeliveryFeesRequestModel requestModel);
    }
}
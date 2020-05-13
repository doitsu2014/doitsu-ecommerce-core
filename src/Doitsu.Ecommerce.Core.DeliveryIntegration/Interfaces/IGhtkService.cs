using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.DeliveryIntegration.Common;
using Optional;

namespace Doitsu.Ecommerce.Core.DeliveryIntegration.Interfaces
{
    public interface IGhtkService
    {
        Task<Option<dynamic, string>> CalculateFees(CalculateDeliveryFeesRequestModel requestModel);
    }
}
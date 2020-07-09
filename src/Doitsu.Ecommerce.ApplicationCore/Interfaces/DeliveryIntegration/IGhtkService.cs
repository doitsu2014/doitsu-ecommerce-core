using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Models.DeliveryIntegration.RequestModels;
using Optional;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.DeliveryIntegration
{
    public interface IGhtkService
    {
        Task<Option<dynamic, string>> CalculateFees(CalculateDeliveryFeesRequestModel requestModel);
    }
}
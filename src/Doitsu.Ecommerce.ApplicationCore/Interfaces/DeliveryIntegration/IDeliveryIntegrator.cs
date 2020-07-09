using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore.Models.DeliveryIntegration.RequestModels;
using Optional;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces.DeliveryIntegration
{
    public interface IDeliveryIntegrator
    {
       Task<Option<dynamic, string>> CalculateShipFeeAsync(DeliverEnum deliver, CalculateDeliveryFeesRequestModel request); 
    }
}
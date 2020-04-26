using System.Threading.Tasks;
using Doitsu.Ecommerce.DeliveryIntegration.Common;
using Optional;

namespace Doitsu.Ecommerce.DeliveryIntegration
{
    public interface IDeliveryIntegrator
    {
       Task<Option<float, string>> CalculateShipFeeAsync(DeliverEnum deliver, CalculateDeliveryFeesRequestModel request); 
    }
}
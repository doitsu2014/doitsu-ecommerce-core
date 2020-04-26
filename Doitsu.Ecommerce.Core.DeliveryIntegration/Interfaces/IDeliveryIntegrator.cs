using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.DeliveryIntegration.Common;
using Optional;

namespace Doitsu.Ecommerce.Core.DeliveryIntegration
{
    public interface IDeliveryIntegrator
    {
       Task<Option<dynamic, string>> CalculateShipFeeAsync(DeliverEnum deliver, CalculateDeliveryFeesRequestModel request); 
    }
}
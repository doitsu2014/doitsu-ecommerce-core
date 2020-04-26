using System.Threading.Tasks;
using Doitsu.Ecommerce.DeliveryIntegration.Common;
using Optional;

namespace Doitsu.Ecommerce.DeliveryIntegration
{
    public class DeliveryIntegrator : IDeliveryIntegrator
    {
        public DeliveryIntegrator()
        {
        }

        public Task<Option<float, string>> CalculateShipFeeAsync(DeliverEnum deliver, CalculateDeliveryFeesRequestModel request)
        {
            throw new System.NotImplementedException();
        }
    }
}
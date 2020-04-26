using System.Threading.Tasks;
using Doitsu.Ecommerce.Core.DeliveryIntegration.Common;
using Doitsu.Ecommerce.Core.DeliveryIntegration.Interfaces;
using Optional;

namespace Doitsu.Ecommerce.Core.DeliveryIntegration
{
    public class DeliveryIntegrator : IDeliveryIntegrator
    {
        private IGhtkService ghtkService;
        public DeliveryIntegrator(IGhtkService ghtkService)
        {
            this.ghtkService = ghtkService;
        }

        public async Task<Option<dynamic, string>> CalculateShipFeeAsync(DeliverEnum deliver, CalculateDeliveryFeesRequestModel request)
        {
            return await ghtkService.CalculateFees(request);
        }
    }
}
using System.Threading.Tasks;
using Doitsu.Ecommerce.ApplicationCore;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.DeliveryIntegration;
using Doitsu.Ecommerce.ApplicationCore.Models.DeliveryIntegration.RequestModels;
using Optional;

namespace Doitsu.Ecommerce.Infrastructure.Services.DeliveryIntegration
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
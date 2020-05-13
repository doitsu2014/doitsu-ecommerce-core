namespace Doitsu.Ecommerce.Core.DeliveryIntegration.Configuration
{
    public class GhtkPartnerConfiguration : BasePartnerConfiguration
    {
        public string CalculateFeesUrl { get => $"{this.ApiUrl}/services/shipment/fee"; }

    }
}
namespace Doitsu.Ecommerce.DeliveryIntegration.Configuration
{
    public class GHTKPartnerConfiguration : BasePartnerConfiguration
    {
        public string CalculateFeesUrl { get => $"{this.ApiUrl}/services/shipment/fee"; }

    }
}
namespace Doitsu.Ecommerce.ApplicationCore.Models.DeliveryIntegration
{
    public class GhtkPartnerConfiguration : BasePartnerConfiguration
    {
        public string CalculateFeesUrl { get => $"{this.ApiUrl}/services/shipment/fee"; }

    }
}
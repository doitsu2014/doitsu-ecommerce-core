namespace Doitsu.Ecommerce.DeliveryIntegration.Configuration
{
    public abstract class BasePartnerConfiguration
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string ApiUrl { get; set; }                    
    }
}
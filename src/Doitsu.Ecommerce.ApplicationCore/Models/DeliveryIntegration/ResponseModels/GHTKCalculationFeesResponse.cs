using Newtonsoft.Json;

namespace Doitsu.Ecommerce.ApplicationCore.Models.DeliveryIntegration.ResponseModels
{
    public class GHTKCalculationFeesResponse : GHTKBaseResponse
    {
        public GHTKCalculationFeesFeeStructure Fee { get; set; }
    }

    public class GHTKCalculationFeesFeeStructure
    {
        public string Name { get; set; }
        public dynamic Fee { get; set; }

        [JsonProperty("insurance_fee")]
        public dynamic InsuranceFee { get; set; }
    }

}
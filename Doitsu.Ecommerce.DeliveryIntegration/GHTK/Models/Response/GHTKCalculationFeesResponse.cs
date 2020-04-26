using Newtonsoft.Json;

namespace Doitsu.Ecommerce.DeliveryIntegration.GHTK.Models.Response
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
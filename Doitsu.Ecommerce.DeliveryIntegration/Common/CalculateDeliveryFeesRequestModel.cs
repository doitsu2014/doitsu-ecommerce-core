namespace Doitsu.Ecommerce.DeliveryIntegration.Common
{
    public class CalculateDeliveryFeesRequestModel
    {
        public string Address { get; set; }
        public string Province { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Street { get; set; }

        public string PickAddress { get; set; }
        public string PickProvince { get; set; }
        public string PickDistrict { get; set; }
        public string PickWard { get; set; }
        public string PickStreet { get; set; }

        public float Weight { get; set; }
        public dynamic OriginValue { get; set; }
    }
}
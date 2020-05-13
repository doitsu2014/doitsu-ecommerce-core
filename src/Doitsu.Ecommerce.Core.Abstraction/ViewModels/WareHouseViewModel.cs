namespace Doitsu.Ecommerce.Core.Abstraction.ViewModels
{
    public class WareHouseViewModel
    {
        public int BrandId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }
    }
}
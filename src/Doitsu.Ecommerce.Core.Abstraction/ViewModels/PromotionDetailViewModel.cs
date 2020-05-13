namespace Doitsu.Ecommerce.Core.Abstraction.ViewModels
{
    public class PromotionDetailViewModel
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public float DiscountPercent { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
    }
}
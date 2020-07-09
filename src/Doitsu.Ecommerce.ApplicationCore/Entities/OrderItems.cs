using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
{
    public class OrderItems : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public int ProductId { get; set; }
        public int? ProductVariantId { get; set; }
        public int OrderId { get; set; }
        public decimal SubTotalPrice { get; set; }
        public int SubTotalQuantity { get; set; }
        public double? Discount { get; set; }
        public decimal SubTotalFinalPrice { get; set; }
        public byte[] Vers { get; set; }
        public bool Active { get; set; }
        public string Note { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Products Product { get; set; }
        public virtual ProductVariants ProductVariant { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
{
    public class ProductVariants : Entity<int>, IConcurrencyCheckVers, IActivable, IAuditable
    {
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public decimal AnotherPrice { get; set; }
        public float AnotherDiscount { get; set; }
        public long InventoryQuantity { get; set; }
        public ProductVariantStatusEnum Status { get; set; }
        public ProductVariantInventoryStatusEnum InventoryStatus { get; set; }
        public string ImageThumbUrl { get; set; }
        public string ImageUrls { get; set; }
        public byte[] Vers { get; set; }
        public bool Active { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual Products Product { get; set; }
        public virtual ICollection<ProductVariantOptionValues> ProductVariantOptionValues { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }
    }
}

using System.Collections.Generic;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class ProductVariants : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public int ProductId { get; set; }
        public string Sku { get; set; }
        public decimal AnotherPrice { get; set; }
        public float AnotherDiscount { get; set; }
        public long InventoryQuantity { get; set; }
        public ProductVariantStatusEnum Status { get; set; }
        public byte[] Vers { get; set; }
        public bool Active { get; set; }

        public virtual Products Product { get; set; }
        public virtual ICollection<ProductVariantOptionValues> ProductVariantOptionValues { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }
    }
}

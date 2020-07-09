using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
{
    public class ProductOptions : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Name { get; set; }
        public int ProductId { get; set; }
        public byte[] Vers { get; set; }
        public bool Active { get; set; }

        public virtual Products Product { get; set; }
        public virtual ICollection<ProductOptionValues> ProductOptionValues { get; set; }
        public virtual ICollection<ProductVariantOptionValues> ProductVariantOptionValues { get; set; }
    }
}

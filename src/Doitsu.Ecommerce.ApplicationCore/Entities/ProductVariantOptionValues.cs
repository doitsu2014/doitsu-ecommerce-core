using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
{
    public class ProductVariantOptionValues : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public int ProductVariantId  { get; set; }
        public int? ProductOptionValueId { get; set; }
        public int? ProductOptionId { get; set; }
        public byte[] Vers { get; set; }
        public bool Active  { get; set; }

        public virtual ProductOptionValues ProductOptionValue { get; set; }
        public virtual ProductOptions ProductOption { get; set; }
        public virtual ProductVariants ProductVariant { get; set; }
    }
}

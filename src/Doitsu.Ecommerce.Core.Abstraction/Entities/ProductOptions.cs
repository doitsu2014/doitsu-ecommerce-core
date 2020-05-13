using System.Collections.Generic;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Abstraction.Entities
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

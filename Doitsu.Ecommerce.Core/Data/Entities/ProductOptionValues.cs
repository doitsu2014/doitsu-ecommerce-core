using System.Collections.Generic;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class ProductOptionValues : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public int ProductOptionId { get; set; }
        public string Value { get; set; }
        public bool IsSpecial { get; set; }
        public byte[] Vers { get; set; }
        public bool Active  { get; set; }
        public ProductOptionValueStatusEnum Status { get; set; }

        public virtual ProductOptions ProductOption { get; set; }
        public virtual ICollection<ProductVariantOptionValues> ProductVariantOptionValues { get; set; }
    }
}

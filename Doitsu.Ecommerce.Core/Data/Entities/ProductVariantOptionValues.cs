using System.Collections.Generic;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class ProductVariantOptionValues : Entity, IConcurrencyCheckVers
    {
        public int ProductOptionId { get; set; }
        public int ProductVariantId  { get; set; }
        public int ProductOptionValueId { get; set; }
        public byte[] Vers { get; set; }
        public virtual ProductOptionValues ProductOptionValue { get; set; }
        public virtual ProductVariants ProductVariant { get; set; }
        public virtual ProductOptions ProductOption { get; set; }
    }
}

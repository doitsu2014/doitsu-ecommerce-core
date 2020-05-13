using System;
using Doitsu.Ecommerce.Core.Abstraction.Identities;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Abstraction.Entities
{
    public class PromotionDetail : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public float DiscountPercent { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDateTime { get; set; }
        public byte[] Vers { get; set; }

        public int? ProductVariantId { get; set; }
        public virtual ProductVariants ProductVariant { get; set; }
    }
}
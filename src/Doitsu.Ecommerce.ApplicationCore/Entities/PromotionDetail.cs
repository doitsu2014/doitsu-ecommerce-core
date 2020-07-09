using System;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
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
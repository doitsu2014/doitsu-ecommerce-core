using System;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class PromotionDetail : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public int ProductVariantId { get; set; }
        public float DiscountPercent { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }
        
        public ProductVariants ProductVariant { get; set; }
    }
}
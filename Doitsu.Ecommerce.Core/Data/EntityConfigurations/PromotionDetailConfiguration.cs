
using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class PromotionDetailConfiguration : BaseEntityConfiguration<PromotionDetail, int>
    {
        public override void Configure(EntityTypeBuilder<PromotionDetail> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Name).HasMaxLength(255);
            builder.Property(e => e.DiscountPercent).HasDefaultValue(0);

            builder.HasOne(e => e.ProductVariant)
                .WithMany(e => e.PromotionDetails)
                .HasForeignKey(e => e.ProductVariantId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

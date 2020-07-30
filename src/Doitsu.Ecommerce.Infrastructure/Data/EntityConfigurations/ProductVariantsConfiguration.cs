using Doitsu.Ecommerce.ApplicationCore;
using Doitsu.Ecommerce.ApplicationCore.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityConfigurations
{
    public class ProductVariantsConfiguration : BaseEntityConfiguration<ProductVariants, int>
    {
        public override void Configure(EntityTypeBuilder<ProductVariants> builder)
        {
            base.Configure(builder);

            builder
                .HasOne(x => x.Product)
                .WithMany(x => x.ProductVariants)
                .IsRequired()
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(x => x.Sku).IsUnique();
            builder.Property(x => x.Sku).IsRequired().HasMaxLength(256);

            builder.Property(x => x.AnotherDiscount).HasDefaultValue(0);
            builder.Property(x => x.AnotherPrice)
                .HasColumnType("decimal(18,4)")
                .HasDefaultValue(0);

            builder.Property(e => e.ImageThumbUrl).HasMaxLength(1000);
            builder.Property(x => x.InventoryQuantity).HasDefaultValue(0);
            builder.Property(x => x.Status).HasDefaultValue(ProductVariantStatusEnum.Available);
            builder.Property(x => x.InventoryStatus).HasDefaultValue(ProductVariantInventoryStatusEnum.Exist);
        }
    }
}

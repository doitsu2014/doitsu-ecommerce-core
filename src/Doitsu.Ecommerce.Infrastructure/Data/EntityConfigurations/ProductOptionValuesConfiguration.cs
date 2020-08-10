using Doitsu.Ecommerce.ApplicationCore;
using Doitsu.Ecommerce.ApplicationCore.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityConfigurations
{
    public class ProductOptionValuesConfiguration : BaseEntityConfiguration<ProductOptionValues, int>
    {
        public override void Configure(EntityTypeBuilder<ProductOptionValues> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.ProductOption)
                .WithMany(x => x.ProductOptionValues)
                .IsRequired()
                .HasForeignKey(x => x.ProductOptionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(x => x.Value).HasMaxLength(256);
            builder.Property(x => x.Status).HasDefaultValue(ProductOptionValueStatusEnum.Available);
            builder.Property(x => x.IsSpecial).HasDefaultValue(false);
        }
    }
}

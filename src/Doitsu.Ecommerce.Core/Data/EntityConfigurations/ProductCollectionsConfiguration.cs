using Doitsu.Ecommerce.Core.Abstraction.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class ProductCollectionsConfiguration : BaseEntityConfiguration<ProductCollections, int>
    {
        public override void Configure(EntityTypeBuilder<ProductCollections> builder)
        {
            base.Configure(builder);

            builder.HasIndex(e => e.Slug)
                    .HasName("UQ__ProductCollection_Slug")
                    .IsUnique();

            builder.Property(e => e.Name).HasMaxLength(255);

            builder.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.ThumbnailUrl).HasMaxLength(1000);
        }
    }
}

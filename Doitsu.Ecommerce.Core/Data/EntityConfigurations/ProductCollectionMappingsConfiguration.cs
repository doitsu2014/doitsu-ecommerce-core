using Doitsu.Ecommerce.Core.Data.Entities;

using EFCore.Abstractions.EntityConfigurations;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class ProductCollectionMappingsConfiguration : BaseEntityConfiguration<ProductCollectionMappings, int>
    {
        public override void Configure(EntityTypeBuilder<ProductCollectionMappings> builder)
        {
            base.Configure(builder);

            builder.HasIndex(e => new { e.ProductCollectionId, e.ProductId })
                .HasName("UQ__ProductCollectionId__ProductId")
                .IsUnique();

            builder.HasOne(d => d.ProductCollection)
                .WithMany(p => p.ProductCollectionMappings)
                .HasForeignKey(d => d.ProductCollectionId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__ProductCollection__ProductCollMapping");

            builder.HasOne(d => d.Product)
                .WithMany(p => p.ProductCollectionMappings)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Product__ProductCollMapping");
        }
    }
}
using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class ProductsConfiguration : BaseEntityConfiguration<Products, int>
    {
        public override void Configure(EntityTypeBuilder<Products> builder)
        {
            base.Configure(builder);
            builder.HasIndex(e => e.ArtistId);

            builder.HasIndex(e => e.CateId);

            builder.HasIndex(e => e.CollectionId);

            builder.HasIndex(e => e.Name)
                .HasName("NonClusteredIndex-20190831-223127");

            builder.HasIndex(e => e.Slug)
                .HasName("NonClusteredIndex-20190831-223149");

            builder.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.ImageThumbUrl).HasMaxLength(1000);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Price).HasColumnType("money");

            builder.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Weight)
                .HasDefaultValue(0);

            builder.HasOne(d => d.Cate)
                .WithMany(p => p.Products)
                .HasForeignKey(d => d.CateId);
        }
    }
}

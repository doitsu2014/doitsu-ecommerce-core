using Doitsu.Ecommerce.ApplicationCore.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityConfigurations
{
    public class ProductTagConfiguration : BaseEntityConfiguration<ProductTag, int>
    {
        public override void Configure(EntityTypeBuilder<ProductTag> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Id).ValueGeneratedNever();

                builder.HasOne(d => d.Product)
                    .WithMany(p => p.ProductTag)
                    .HasForeignKey(d => d.ProductId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__ProductTa__Produ__14270015");

                builder.HasOne(d => d.Tag)
                    .WithMany(p => p.ProductTag)
                    .HasForeignKey(d => d.TagId)
                    .IsRequired()
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__ProductTa__TagId__1332DBDC");
        }
    }
}

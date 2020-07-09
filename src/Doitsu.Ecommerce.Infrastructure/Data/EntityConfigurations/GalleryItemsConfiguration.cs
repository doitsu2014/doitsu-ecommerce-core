using Doitsu.Ecommerce.ApplicationCore.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityConfigurations
{
    public class GalleryItemsConfiguration : BaseEntityConfiguration<GalleryItems, int>
    {
        public override void Configure(EntityTypeBuilder<GalleryItems> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(255);

            builder.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(e => e.Link)
              .IsRequired()
              .HasMaxLength(255);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(d => d.Gallery)
                .WithMany(p => p.GalleryItems)
                .HasForeignKey(d => d.GalleryId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__GalleryIt__Galle__0D7A0286");
        }
    }
}

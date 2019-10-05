using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class GalleriesConfiguration : BaseEntityConfiguration<Galleries, int>
    {
        public override void Configure(EntityTypeBuilder<Galleries> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Slug)
                  .IsRequired()
                  .HasMaxLength(255);

            builder.Property(e => e.ThumbnailUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(d => d.ParentGallery)
                .WithMany(p => p.InverseParentGallery)
                .HasForeignKey(d => d.ParentGalleryId)
                .HasConstraintName("FK__Galleries__Paren__0C85DE4D");
        }
    }
}

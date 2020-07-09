using Doitsu.Ecommerce.ApplicationCore.Entities;

using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityConfigurations
{
    public class CataloguesConfiguration : BaseEntityConfiguration<Catalogues, int>
    {
        public override void Configure(EntityTypeBuilder<Catalogues> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.ImageUrl)
                    .IsRequired()
                    .HasMaxLength(1000);

            builder.Property(e => e.PdfUrl)
                .IsRequired()
                .HasMaxLength(1000);

            builder.Property(e => e.ReferenceUrl).HasMaxLength(255);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}

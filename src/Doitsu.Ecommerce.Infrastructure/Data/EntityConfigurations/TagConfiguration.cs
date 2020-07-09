
using Doitsu.Ecommerce.ApplicationCore.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityConfigurations
{
    public class TagConfiguration : BaseEntityConfiguration<Tag, int>
    {
        public override void Configure(EntityTypeBuilder<Tag> builder)
        {
            base.Configure(builder);
            builder.HasIndex(e => e.Title)
                   .HasName("IX_Title");

            builder.Property(e => e.Slug)
                .IsRequired()
                .HasDefaultValueSql("('')");

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(128);
        }
    }
}

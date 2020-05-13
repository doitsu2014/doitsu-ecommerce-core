
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class SlidersConfiguration : BaseEntityConfiguration<Sliders, int>
    {
        public override void Configure(EntityTypeBuilder<Sliders> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.ImageUrl)
                   .IsRequired()
                   .HasMaxLength(1000);

            builder.Property(e => e.ReferenceUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.Slogan)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.Title)
                .IsRequired()
                .HasMaxLength(50);
        }
    }
}

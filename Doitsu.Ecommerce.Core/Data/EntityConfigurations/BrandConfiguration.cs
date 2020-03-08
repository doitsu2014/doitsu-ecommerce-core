using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class BrandConfiguration : BaseEntityConfiguration<Brand, int>
    {
        public override void Configure(EntityTypeBuilder<Brand> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Address).HasMaxLength(400);

            builder.Property(e => e.AlternativeAddress).HasMaxLength(400);

            builder.Property(e => e.Description).HasMaxLength(800);

            builder.Property(e => e.FacebookScript).HasMaxLength(1000);

            builder.Property(e => e.FacebookUrl).HasMaxLength(400);

            builder.Property(e => e.Fax).HasMaxLength(20);

            builder.Property(e => e.GooglePlusUrl).HasMaxLength(400);

            builder.Property(e => e.GoogleScript).HasMaxLength(1000);

            builder.Property(e => e.HotLine).HasMaxLength(50);

            builder.Property(e => e.InstagramUrl).HasColumnName("InstagramURL");

            builder.Property(e => e.LinkedInUrl).HasMaxLength(400);

            builder.Property(e => e.LogoRectangleUrl).HasMaxLength(400);

            builder.Property(e => e.LogoSquareUrl).HasMaxLength(400);

            builder.Property(e => e.Mail).HasMaxLength(255);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.TwitterUrl).HasMaxLength(400);

            builder.Property(e => e.YoutubeUrl).HasMaxLength(400);
            builder.Property(e => e.FaviconUrl).HasMaxLength(1000);
        }
    }
}

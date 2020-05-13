using Doitsu.Ecommerce.Core.Abstraction.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class BrandFeedbacksConfiguration : BaseEntityConfiguration<BrandFeedbacks, int>
    {
        public override void Configure(EntityTypeBuilder<BrandFeedbacks> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Id).ValueGeneratedNever();

            builder.Property(e => e.AvatarUrl)
                .IsRequired()
                .HasMaxLength(400);

            builder.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(400);

            builder.Property(e => e.Email).HasMaxLength(50);

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasOne(d => d.User)
                .WithMany(p => p.BrandFeedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__BrandFeed__UserI__09A971A2");
        }
    }
}

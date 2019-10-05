using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class CustomerFeedbacksConfiguration : BaseEntityConfiguration<CustomerFeedbacks, int>
    {
        public override void Configure(EntityTypeBuilder<CustomerFeedbacks> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Address).HasMaxLength(300);

            builder.Property(e => e.Content)
                .IsRequired()
                .HasMaxLength(800);

            builder.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            builder.Property(e => e.CustomerName)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(255);

            builder.Property(e => e.Phone)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasOne(d => d.User)
                .WithMany(p => p.CustomerFeedbacks)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__CustomerF__UserI__0B27A5C0");
        }
    }
}

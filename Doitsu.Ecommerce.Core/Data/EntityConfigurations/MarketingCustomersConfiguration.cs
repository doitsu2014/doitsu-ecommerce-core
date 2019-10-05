using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class MarketingCustomersConfiguration : BaseEntityConfiguration<MarketingCustomers, int>
    {
        public override void Configure(EntityTypeBuilder<MarketingCustomers> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(255);

            builder.Property(e => e.JoinedDate).HasDefaultValueSql("(getdate())");

            builder.HasOne(d => d.User)
                .WithMany(p => p.MarketingCustomers)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Marketing__UserI__5B78929E");
        }
    }
}

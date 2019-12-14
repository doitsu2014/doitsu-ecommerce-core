
using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class UserTransactionConfiguration : BaseEntityConfiguration<UserTransaction, int>
    {
        public override void Configure(EntityTypeBuilder<UserTransaction> builder)
        {
            base.Configure(builder);
            builder.Property(e => e.Description).HasMaxLength(500);
            builder.Property(e => e.Amount).HasDefaultValue(0);
            builder.Property(e => e.CreatedTime).HasDefaultValueSql("(getutcdate())");

            builder.HasOne(e => e.User)
                .WithMany(e => e.UserTransactions)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(e => e.Order)
                .WithMany(e => e.UserTransactions)
                .HasForeignKey(e => e.OrderId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}

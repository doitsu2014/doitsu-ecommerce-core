using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class OrdersConfiguration : BaseEntityConfiguration<Orders, int>
    {
        public override void Configure(EntityTypeBuilder<Orders> builder)
        {
            base.Configure(builder);

            builder.HasIndex(e => e.Code)
                    .HasName("UQ_Code")
                    .IsUnique();

            builder.Property(e => e.Code)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(e => e.CreatedDate).HasDefaultValueSql("(getutcdate())");

            builder.Property(e => e.DeliveryAddress).HasMaxLength(300);

            builder.Property(e => e.DeliveryEmail).HasMaxLength(255);

            builder.Property(e => e.DeliveryName).HasMaxLength(255);

            builder.Property(e => e.DeliveryPhone).HasMaxLength(20);

            builder.Property(e => e.FinalPrice).HasColumnType("money");

            builder.Property(e => e.TotalPrice).HasColumnType("money");

            builder.HasOne(d => d.User)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__UserId__114A936A");
        }
    }
}

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

            builder.Property(e => e.DeliveryAddress).HasMaxLength(255);

            builder.Property(e => e.DeliveryEmail).HasMaxLength(125);

            builder.Property(e => e.DeliveryName).HasMaxLength(125);

            builder.Property(e => e.DeliveryPhone).HasMaxLength(20);

            builder.Property(e => e.DeliveryCity).HasMaxLength(60);

            builder.Property(e => e.DeliveryDistrict).HasMaxLength(30);

            builder.Property(e => e.DeliveryWard).HasMaxLength(30);

            builder.Property(e => e.FinalPrice).HasColumnType("money");

            builder.Property(e => e.TotalPrice).HasColumnType("money");

            builder.Property(e => e.Dynamic01).HasMaxLength(255);

            builder.Property(e => e.Dynamic02).HasMaxLength(255);

            builder.Property(e => e.Dynamic03).HasMaxLength(255);

            builder.Property(e => e.Dynamic04).HasMaxLength(255);

            builder.Property(e => e.Dynamic05).HasMaxLength(255);

            builder.Property(e => e.Note).HasMaxLength(500);

            builder.Property(e => e.CancelNote).HasMaxLength(255);

            builder.Property(e => e.PaymentProofImageUrl).HasMaxLength(300);

            builder.Property(e => e.Status).HasDefaultValue(OrderTypeEnum.Sale);

            builder.HasOne(d => d.User)
                .WithMany(p => p.Orders)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Orders__UserId__114A936A");

            builder.HasOne(o => o.SummaryOrder)
                .WithMany(o => o.InverseSummaryOrders)
                .HasForeignKey(o => o.SummaryOrderId);

            builder.HasOne(o => o.RefernceDeliveryInformation)
                .WithMany(addr => addr.Orders)
                .HasForeignKey(o => o.RefernceDeliveryInformationId)
                .OnDelete(DeleteBehavior.ClientSetNull);

        }
    }
}

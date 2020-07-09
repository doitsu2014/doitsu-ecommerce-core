﻿using Doitsu.Ecommerce.ApplicationCore.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityConfigurations
{
    public class OrderItemsConfiguration : BaseEntityConfiguration<OrderItems, int>
    {
        public override void Configure(EntityTypeBuilder<OrderItems> builder)
        {
            base.Configure(builder);

            builder.Property(e => e.SubTotalFinalPrice).HasColumnType("money");
            builder.Property(e => e.SubTotalPrice).HasColumnType("money");
            builder.Property(e => e.Note).HasMaxLength(500);

            builder.HasOne(d => d.Order)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.OrderId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__OrderItem__Order__10566F31");

            builder.HasOne(d => d.Product)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__OrderItem__Produ__0F624AF8");

            builder.HasOne(d => d.ProductVariant)
                .WithMany(p => p.OrderItems)
                .HasForeignKey(d => d.ProductVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}

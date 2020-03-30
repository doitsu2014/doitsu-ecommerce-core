using System.ComponentModel;
using System;
using System.Linq.Expressions;
using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class DeliveryInformationConfiguration : BaseEntityConfiguration<DeliveryInformation, int>
    {
        public override void Configure(EntityTypeBuilder<DeliveryInformation> builder)
        {
            base.Configure(builder);
            builder.Property(di => di.Address).HasMaxLength(255);
            builder.Property(di => di.Name).HasMaxLength(125);
            builder.Property(di => di.Phone).HasMaxLength(20);
            builder.Property(di => di.State).HasMaxLength(30);
            builder.Property(di => di.ZipCode).HasMaxLength(15);
            builder.Property(di => di.Country).HasMaxLength(60);
            builder.Property(di => di.Email).HasMaxLength(255);

            builder.HasOne(addr => addr.User)
                .WithMany(u => u.DeliveryInformations)
                .HasForeignKey(addr => addr.UserId)
                .IsRequired();
        }
    }
}
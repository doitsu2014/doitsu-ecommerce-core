using System;
using System.Linq.Expressions;
using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class ProductOptionsConfiguration : BaseEntityConfiguration<ProductOptions, int>
    {
        public override void Configure(EntityTypeBuilder<ProductOptions> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Name)
                .HasMaxLength(256)
                .IsRequired();

            builder.HasOne(x => x.Product)
                .WithMany(x => x.ProductOptions)
                .IsRequired()
                .HasForeignKey(x => x.ProductId);
        }
    }
}

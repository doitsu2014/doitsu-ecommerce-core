using System;
using System.Linq.Expressions;
using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class ProductVariantOptionValuesConfiguration : BaseEntityConfiguration<ProductVariantOptionValues, int>
    {
        public override void Configure(EntityTypeBuilder<ProductVariantOptionValues> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.ProductVariant)
                .WithMany(x => x.ProductVariantOptionValues)
                .HasForeignKey(x => x.ProductVariantId)
                .IsRequired();

            builder.HasOne(x => x.ProductOptionValue)
                .WithMany(x => x.ProductVariantOptionValues)
                .HasForeignKey(x => x.ProductOptionValueId);

            builder.HasOne(x => x.ProductOption)
                .WithMany(x => x.ProductVariantOptionValues)
                .HasForeignKey(x => x.ProductOptionId);
        }
    }
}

using System;
using System.Linq.Expressions;
using Doitsu.Ecommerce.ApplicationCore.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityConfigurations
{
    public class ProductVariantOptionValuesConfiguration : BaseEntityConfiguration<ProductVariantOptionValues, int>
    {
        public override void Configure(EntityTypeBuilder<ProductVariantOptionValues> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.ProductVariant)
                .WithMany(x => x.ProductVariantOptionValues)
                .HasForeignKey(x => x.ProductVariantId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // builder.HasOne(x => x.ProductOptionValue)
            //     .WithMany(x => x.ProductVariantOptionValues)
            //     .HasForeignKey(x => x.ProductOptionValueId)
            //     .OnDelete(DeleteBehavior.Cascade);

            // builder.HasOne(x => x.ProductOption)
            //     .WithMany(x => x.ProductVariantOptionValues)
            //     .HasForeignKey(x => x.ProductOptionId)
            //     .OnDelete(DeleteBehavior.ClientCascade);
        }
    }
}

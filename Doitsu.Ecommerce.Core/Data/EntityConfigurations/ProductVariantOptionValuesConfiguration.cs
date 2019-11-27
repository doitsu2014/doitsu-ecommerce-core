using System;
using System.Linq.Expressions;
using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class ProductVariantOptionValuesConfiguration : BaseEntityConfiguration<ProductVariantOptionValues>
    {
        public override Expression<Func<ProductVariantOptionValues, object>> KeyExpression => x => new { x.ProductOptionId, x.ProductVariantId };

        public override void Configure(EntityTypeBuilder<ProductVariantOptionValues> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.ProductOption)
                .WithMany(x => x.ProductVariantOptionValues)
                .HasForeignKey(x => x.ProductOptionId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(x => x.ProductVariant)
                .WithMany(x => x.ProductVariantOptionValues)
                .HasForeignKey(x => x.ProductVariantId)
                .OnDelete(DeleteBehavior.ClientSetNull);

            builder.HasOne(x => x.ProductOptionValue)
                .WithMany(x => x.ProductVariantOptionValues)
                .HasForeignKey(x => x.ProductOptionValueId)
                .OnDelete(DeleteBehavior.ClientSetNull);
         
        }
    }
}

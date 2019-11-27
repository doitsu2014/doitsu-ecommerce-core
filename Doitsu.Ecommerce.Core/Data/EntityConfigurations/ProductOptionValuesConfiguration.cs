using System;
using System.Linq.Expressions;
using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class ProductOptionValuesConfiguration : BaseEntityConfiguration<ProductOptionValues, int>
    {
        public override void Configure(EntityTypeBuilder<ProductOptionValues> builder)
        {
            base.Configure(builder);

            builder.HasOne(x => x.ProductOption)
                .WithMany(x => x.ProductOptionValues)
                .HasForeignKey(x => x.ProductOptionId);

            builder.Property(x => x.Value).HasMaxLength(256);
            builder.Property(x => x.Status).HasDefaultValue(ProductOptionValueStatusEnum.Available);
        }
    }
}

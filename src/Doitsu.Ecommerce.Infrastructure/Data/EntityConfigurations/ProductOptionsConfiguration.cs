using System;
using System.Linq.Expressions;
using Doitsu.Ecommerce.ApplicationCore.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityConfigurations
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
                .HasForeignKey(x => x.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

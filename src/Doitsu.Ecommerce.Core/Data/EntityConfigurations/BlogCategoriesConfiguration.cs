using System;
using System.Linq.Expressions;
using Doitsu.Ecommerce.Core.Abstraction.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class BlogCategoriesConfiguration : BaseEntityConfiguration<BlogCategories, int>
    {
        public override void Configure(EntityTypeBuilder<BlogCategories> builder)
        {
            base.Configure(builder);
            builder.HasIndex(e => e.Slug)
                .HasName("IDX_BC_SLUG");
                
            builder.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(e => e.Name).HasMaxLength(500);

            builder.Property(e => e.UpdatedTime).HasDefaultValueSql("(getdate())");
            builder.Property(e => e.CreatedDate).HasDefaultValueSql("(getdate())");

            builder.Property(e => e.Vers)
                .IsRequired()
                .IsRowVersion();

            builder.HasOne(d => d.BlogCategory)
                .WithMany(p => p.InverseBlogCategory)
                .HasForeignKey(d => d.BlogCategoryId)
                .HasConstraintName("FK__BlogCateg__BlogC__03F0984C");
        }
    }
}

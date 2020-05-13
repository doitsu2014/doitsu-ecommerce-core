using Doitsu.Ecommerce.Core.Abstraction.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class CategoriesConfiguration : BaseEntityConfiguration<Categories, int>
    {
        public override void Configure(EntityTypeBuilder<Categories> builder)
        {
            base.Configure(builder);
            builder.HasIndex(e => e.Slug)
                   .HasName("UQ__Categori__BC7B5FB6B83DB6AB")
                   .IsUnique();

            builder.Property(e => e.Name).HasMaxLength(255);

            builder.Property(e => e.Slug)
                .IsRequired()
                .HasMaxLength(255);

            builder.HasOne(d => d.ParentCate)
                .WithMany(p => p.InverseParentCate)
                .HasForeignKey(d => d.ParentCateId)
                .HasConstraintName("FK__Categorie__Paren__0A9D95DB");
        }
    }
}

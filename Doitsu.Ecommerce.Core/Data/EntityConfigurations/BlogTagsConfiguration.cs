using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class BlogTagsConfiguration : BaseEntityConfiguration<BlogTags, int>
    {
        public override void Configure(EntityTypeBuilder<BlogTags> builder)
        {
            base.Configure(builder);

            builder.HasOne(d => d.Blog)
                .WithMany(p => p.BlogTags)
                .HasForeignKey(d => d.BlogId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__BlogTags__BlogId__07C12930");

            builder.HasOne(d => d.Tag)
                .WithMany(p => p.BlogTags)
                .HasForeignKey(d => d.TagId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK__BlogTags__TagId__08B54D69");
        }
    }
}

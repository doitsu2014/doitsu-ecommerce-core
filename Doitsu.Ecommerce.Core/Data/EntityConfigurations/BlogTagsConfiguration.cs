using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class BlogTagsConfiguration : BaseEntityConfiguration<BlogTags, int>
    {
        public override void Configure(EntityTypeBuilder<BlogTags> builder)
        {
            base.Configure(builder);
        }
    }
}

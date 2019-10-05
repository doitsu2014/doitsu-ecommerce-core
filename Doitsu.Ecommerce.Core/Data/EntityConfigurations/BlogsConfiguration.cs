using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class BlogsConfiguration : BaseEntityConfiguration<Blogs, int>
    {
        public override void Configure(EntityTypeBuilder<Blogs> builder)
        {
            base.Configure(builder);
        }
    }
}

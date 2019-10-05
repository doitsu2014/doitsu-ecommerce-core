using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class ProductTagConfiguration : BaseEntityConfiguration<ProductTag, int>
    {
        public override void Configure(EntityTypeBuilder<ProductTag> builder)
        {
            base.Configure(builder);
        }
    }
}

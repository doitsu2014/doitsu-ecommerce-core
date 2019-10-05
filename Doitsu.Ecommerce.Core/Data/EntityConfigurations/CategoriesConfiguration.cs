using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class CategoriesConfiguration : BaseEntityConfiguration<Categories, int>
    {
        public override void Configure(EntityTypeBuilder<Categories> builder)
        {
            base.Configure(builder);
        }
    }
}

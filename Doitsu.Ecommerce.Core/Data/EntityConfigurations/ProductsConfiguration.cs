using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class ProductsConfiguration : BaseEntityConfiguration<Products, int>
    {
        public override void Configure(EntityTypeBuilder<Products> builder)
        {
            base.Configure(builder);
        }
    }
}

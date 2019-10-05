using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class OrderItemsConfiguration : BaseEntityConfiguration<OrderItems, int>
    {
        public override void Configure(EntityTypeBuilder<OrderItems> builder)
        {
            base.Configure(builder);
        }
    }
}

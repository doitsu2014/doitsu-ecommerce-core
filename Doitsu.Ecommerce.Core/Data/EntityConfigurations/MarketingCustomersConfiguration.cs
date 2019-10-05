using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class MarketingCustomersConfiguration : BaseEntityConfiguration<MarketingCustomers, int>
    {
        public override void Configure(EntityTypeBuilder<MarketingCustomers> builder)
        {
            base.Configure(builder);
        }
    }
}

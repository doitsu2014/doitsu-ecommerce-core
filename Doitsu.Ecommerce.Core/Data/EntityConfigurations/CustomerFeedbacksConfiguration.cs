using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class CustomerFeedbacksConfiguration : BaseEntityConfiguration<CustomerFeedbacks, int>
    {
        public override void Configure(EntityTypeBuilder<CustomerFeedbacks> builder)
        {
            base.Configure(builder);
        }
    }
}

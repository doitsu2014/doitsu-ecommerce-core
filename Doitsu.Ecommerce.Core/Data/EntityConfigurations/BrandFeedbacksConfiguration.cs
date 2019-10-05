using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class BrandFeedbacksConfiguration : BaseEntityConfiguration<BrandFeedbacks, int>
    {
        public override void Configure(EntityTypeBuilder<BrandFeedbacks> builder)
        {
            base.Configure(builder);
        }
    }
}

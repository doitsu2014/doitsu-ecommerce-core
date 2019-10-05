
using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class SlidersConfiguration : BaseEntityConfiguration<Sliders, int>
    {
        public override void Configure(EntityTypeBuilder<Sliders> builder)
        {
            base.Configure(builder);
        }
    }
}

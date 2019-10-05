using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class GalleriesConfiguration : BaseEntityConfiguration<Galleries, int>
    {
        public override void Configure(EntityTypeBuilder<Galleries> builder)
        {
            base.Configure(builder);
        }
    }
}

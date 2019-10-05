using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class CataloguesConfiguration : BaseEntityConfiguration<Catalogues, int>
    {
        public override void Configure(EntityTypeBuilder<Catalogues> builder)
        {
            base.Configure(builder);
        }
    }
}

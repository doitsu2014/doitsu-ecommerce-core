using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class GalleryItemsConfiguration : BaseEntityConfiguration<GalleryItems, int>
    {
        public override void Configure(EntityTypeBuilder<GalleryItems> builder)
        {
            base.Configure(builder);
        }
    }
}

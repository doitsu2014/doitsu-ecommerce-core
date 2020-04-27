
using Doitsu.Ecommerce.Core.Data.Entities;
using EFCore.Abstractions.EntityConfigurations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Core.Data.EntityConfigurations
{
    public class WareHouseConfiguration : BaseEntityConfiguration<WareHouse, int>
    {
        public override void Configure(EntityTypeBuilder<WareHouse> builder)
        {
            base.Configure(builder);
            builder.HasIndex(e => e.Name)
                   .HasName("IX_Name");

            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(128);
            builder.Property(e => e.District)
                .IsRequired()
                .HasMaxLength(64);
            builder.Property(e => e.City)
                .IsRequired()
                .HasMaxLength(64);
            builder.Property(e => e.Ward)
                .IsRequired()
                .HasMaxLength(32);
            builder.Property(e => e.Address)
                .IsRequired()
                .HasMaxLength(256);
            
            builder.HasOne(wh => wh.Brand)
                .WithMany(b => b.WareHouses)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}

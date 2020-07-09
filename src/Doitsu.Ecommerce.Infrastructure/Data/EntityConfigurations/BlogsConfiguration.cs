using Doitsu.Ecommerce.ApplicationCore.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityConfigurations
{
    public class BlogsConfiguration : BaseEntityConfiguration<Blogs, int>
    {
        public override void Configure(EntityTypeBuilder<Blogs> builder)
        {
            base.Configure(builder);
             builder.HasIndex(e => e.Slug)
                    .HasName("IDX_B_SLUG");

                builder.Property(e => e.Content).IsRequired();

                builder.Property(e => e.DraftedTime).HasDefaultValueSql("(getdate())");

                builder.Property(e => e.ShortContent).HasMaxLength(500);

                builder.Property(e => e.Slug)
                    .IsRequired()
                    .HasMaxLength(250);

                builder.Property(e => e.ThumbnailUrl).HasMaxLength(1000);

                builder.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(250);

                builder.HasOne(d => d.BlogCategory)
                    .WithMany(p => p.Blogs)
                    .HasForeignKey(d => d.BlogCategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Blogs__BlogCateg__04E4BC85");

                builder.HasOne(d => d.Creater)
                    .WithMany(p => p.BlogsCreater)
                    .HasForeignKey(d => d.CreaterId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Blogs__CreaterId__06CD04F7");

                builder.HasOne(d => d.Publisher)
                    .WithMany(p => p.BlogsPublisher)
                    .HasForeignKey(d => d.PublisherId)
                    .HasConstraintName("FK__Blogs__Publisher__05D8E0BE");
        }
    }
}

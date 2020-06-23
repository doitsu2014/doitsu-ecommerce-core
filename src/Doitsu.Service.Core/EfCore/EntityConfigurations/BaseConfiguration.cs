using System;
using System.Linq.Expressions;
using Doitsu.Service.Core.Interfaces.EfCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EFCore.Abstractions.EntityConfigurations
{
    public abstract class BaseConfiguration<T> : IEntityTypeConfiguration<T> where T : class
    {
        private const string DefaultSchema = "dbo";

        public abstract Expression<Func<T, object>> KeyExpression { get; }

        public virtual string Schema => DefaultSchema;

        public virtual void Configure(EntityTypeBuilder<T> builder)
        {
            builder.HasKey(KeyExpression);

            if (Schema != DefaultSchema)
                builder.ToTable(typeof(T).Name, Schema);

            // Ignore all soft-deleted entities, use Set<T>().IgnoreQueryFilters() if we want
            // to include them in the query
            if (typeof(ISoftDeletable).IsAssignableFrom(typeof(T)))
            {
                builder.HasQueryFilter(p => !((ISoftDeletable)p).Deleted);
            }

            if (typeof(IActivable).IsAssignableFrom(typeof(T)))
            {
                builder.HasQueryFilter(p => ((IActivable)p).Active);
                builder.Property(p => ((IActivable)p).Active).HasDefaultValue(true);
            }

            if (typeof(IConcurrencyCheckVers).IsAssignableFrom(typeof(T)))
            {
                builder.Property(p => ((IConcurrencyCheckVers)p).Vers).IsRequired().IsRowVersion();
            }

            if (typeof(IAuditable).IsAssignableFrom(typeof(T)))
            {
                // builder.Property(p => ((IAuditable)p).CreatedDate).HasDefaultValueSql("(getdate())");
                // builder.Property(p => ((IAuditable)p).LastUpdatedDate).HasDefaultValueSql("(getdate())");
            }
        }
    }
}

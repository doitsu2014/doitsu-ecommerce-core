using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityConfigurations
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

            // Ignore all IActivable entities, use Set<T>().IgnoreQueryFilters() if we want
            // to include them in the query
            if (typeof(IActivable).IsAssignableFrom(typeof(T)))
            {
                builder.HasQueryFilter(p => ((IActivable)p).Active);
                builder.Property(p => ((IActivable)p).Active).HasDefaultValue(true);
            }

            if (typeof(IConcurrencyCheckVers).IsAssignableFrom(typeof(T)))
            {
                builder.Property(p => ((IConcurrencyCheckVers)p).Vers).IsRequired().IsRowVersion();
            }
        }
    }
}

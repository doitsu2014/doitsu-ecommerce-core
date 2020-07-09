using Doitsu.Ecommerce.ApplicationCore;
using System;
using System.Linq.Expressions;

namespace Doitsu.Ecommerce.Infrastructure.Data.EntityConfigurations
{
    public abstract class BaseEntityConfiguration<T> : BaseConfiguration<T> where T : Entity { }

    public abstract class BaseEntityConfiguration<T, TId> : BaseEntityConfiguration<T> where T : Entity<TId>
    {
        public override Expression<Func<T, object>> KeyExpression => x => x.Id;
    }
}

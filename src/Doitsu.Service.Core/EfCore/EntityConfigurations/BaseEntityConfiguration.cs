using System;
using System.Linq.Expressions;
using EFCore.Abstractions.Models;

namespace EFCore.Abstractions.EntityConfigurations
{
    public abstract class BaseEntityConfiguration<T> : BaseConfiguration<T> where T : Entity { }

    public abstract class BaseEntityConfiguration<T, TId> : BaseEntityConfiguration<T> where T : Entity<TId>
    {
        public override Expression<Func<T, object>> KeyExpression => x => x.Id;
    }
}

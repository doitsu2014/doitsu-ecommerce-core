using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Doitsu.Ecommerce.ApplicationCore.Interfaces
{
    public interface ISpecification<T, TResult> : ISpecification<T>
    {
        #nullable enable
        Expression<Func<T, TResult>>? Selector { get; set; }
        #nullable disable
    }

    public interface ISpecification<T>
    {
        bool CacheEnabled { get; }

        #nullable enable
        string? CacheKey { get; }
        Expression<Func<T, object>>? OrderBy { get; }
        Expression<Func<T, object>>? ThenBy { get; }
        Expression<Func<T, object>>? OrderByDescending { get; }
        #nullable disable

        IEnumerable<Expression<Func<T, bool>>> Criterias { get; }
        IEnumerable<Expression<Func<T, object>>> Includes { get; }
        IEnumerable<string> IncludeStrings { get; }

        //Expression<Func<T, object>> GroupBy { get; }

        int Take { get; }
        int Skip { get; }
        bool IsPagingEnabled { get; }
    }
}

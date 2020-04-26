using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Doitsu.Ecommerce.Core.Extensions
{
    public static class IQueryableExtension
    {
        public static IIncludableQueryable<T, ICollection<T>> IncludeByDepth<T>(this IQueryable<T> query, Expression<Func<T, ICollection<T>>> lamda, int depth = 1) where T : class
        {
            var includeQuery = query.Include(lamda);

            for (var i = 0; i < depth; ++i)
            {
                includeQuery = includeQuery.ThenInclude(lamda);
            }

            return includeQuery;
        }
    }
}

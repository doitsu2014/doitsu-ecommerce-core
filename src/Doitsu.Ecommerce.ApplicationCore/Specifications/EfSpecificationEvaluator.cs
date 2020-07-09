using System.Linq;
using Doitsu.Ecommerce.ApplicationCore.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications
{
    public class EfSpecificationEvaluator<T, TResult> where T : class
    {
        public static IQueryable<TResult> GetQuery(IQueryable<T> inputQuery, ISpecification<T, TResult> specification)
        {
            var query = EfSpecificationEvaluator<T>.GetQuery(inputQuery, specification);

            // Apply selector
            var selectQuery = query.Select(specification.Selector);

            return selectQuery;
        }
    }

    public class EfSpecificationEvaluator<T> where T : class
    {
        public static IQueryable<T> GetQuery(IQueryable<T> inputQuery, ISpecification<T> specification)
        {
            var query = inputQuery;

            query = SpecificationEvaluator<T>.GetQuery(query, specification);

            // Includes all expression-based includes
            query = specification.Includes.Aggregate(query,
                                    (current, include) => current.Include(include));

            // Include any string-based include statements
            query = specification.IncludeStrings.Aggregate(query,
                                    (current, include) => current.Include(include));

            return query;
        }
    }
}

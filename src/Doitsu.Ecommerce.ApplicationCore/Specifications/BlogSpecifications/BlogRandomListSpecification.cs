using Doitsu.Ecommerce.ApplicationCore.Entities;
using System;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.BlogSpecifications
{
    public sealed class BlogRandomListSpecification : BaseSpecification<Blogs>
    {
        public BlogRandomListSpecification(int skip, int take)
        {
            ApplyPaging(skip, take);
            ApplyOrderBy(b => Guid.NewGuid());
        }
    }
}

using Doitsu.Ecommerce.ApplicationCore.Entities;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.BlogSpecifications
{
    public sealed class BlogFilterPaginatedSpecification : BaseSpecification<Blogs>
    {
        public BlogFilterPaginatedSpecification(int skip, int take, string blogCategorySlug)
            : base(b => b.BlogCategory.Slug == blogCategorySlug)
        {
            ApplyPaging(skip, take);
        }
    }
}

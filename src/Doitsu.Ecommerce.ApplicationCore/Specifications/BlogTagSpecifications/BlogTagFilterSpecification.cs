using Doitsu.Ecommerce.ApplicationCore.Entities;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.BlogTagSpecifications
{
    public sealed class BlogTagFilterSpecification : BaseSpecification<BlogTags>
    {
        public BlogTagFilterSpecification(int blogId) : base(bt => bt.BlogId == blogId)
        { 
        }
    }
}
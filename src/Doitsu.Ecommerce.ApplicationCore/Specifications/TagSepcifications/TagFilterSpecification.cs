using System.Linq;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.QueryExtensions.Include;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.TagSepcifications
{
    public sealed class TagFilterSpecification : BaseSpecification<Tag>
    {
       public TagFilterSpecification(string[] listTagSlugs) : base(t => listTagSlugs.Contains(t.Slug))
       {
       } 
    }
}
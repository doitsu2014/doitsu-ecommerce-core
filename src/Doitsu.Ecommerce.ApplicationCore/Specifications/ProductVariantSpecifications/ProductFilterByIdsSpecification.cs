using System.Linq;
using Doitsu.Ecommerce.ApplicationCore.Entities;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.ProductVariantSpecifications
{
    public class ProductFilterByIdsSpecification : BaseSpecification<Products>
    {
        public ProductFilterByIdsSpecification(int productId)
        {
            AddCriteria(p => productId == p.Id);
            AddInclude(p => p.ProductVariants);
        }

        public ProductFilterByIdsSpecification(int[] productIds)
        {
            AddCriteria(p => productIds.Contains(p.Id));
        }
    }
}
using System.Linq;
using Doitsu.Ecommerce.ApplicationCore.Entities;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.ProductVariantSpecifications
{
    public class ProductFilterByIdsSpecification : BaseSpecification<Products>
    {
        public ProductFilterByIdsSpecification(int[] productIds)
        {
            AddCriteria(p => productIds.Contains(p.Id));
        }
    }
}
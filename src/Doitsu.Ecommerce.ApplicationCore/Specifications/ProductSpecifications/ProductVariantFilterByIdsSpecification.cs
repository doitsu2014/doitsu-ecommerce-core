using System.Linq;
using Doitsu.Ecommerce.ApplicationCore.Entities;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.ProductSpecifications
{
    public class ProductVariantFilterByIdsSpecification : BaseSpecification<ProductVariants>
    {
        public ProductVariantFilterByIdsSpecification(int pvId)
        {
            AddCriteria(pv => pv.Id == pvId);
        }

        public ProductVariantFilterByIdsSpecification(int[] pvIds)
        {
            AddCriteria(pv =>  pvIds.Contains(pv.Id));
        }
    }
}
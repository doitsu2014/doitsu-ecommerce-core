using System.Linq;
using Doitsu.Ecommerce.ApplicationCore.Entities;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.ProductSpecifications
{
    public class ProductFilterByIdsSpecification : BaseSpecification<Products>
    {
        public ProductFilterByIdsSpecification(int productId)
        {
            AddCriteria(p => productId == p.Id);
            AddInclude("ProductOptions.ProductOptionValues");
            AddInclude("ProductVariants.ProductVariantOptionValues");
        }

        public ProductFilterByIdsSpecification(int[] productIds)
        {
            AddCriteria(p => productIds.Contains(p.Id));
            AddInclude("ProductOptions.ProductOptionValues");
            AddInclude("ProductVariants.ProductVariantOptionValues");
        }
    }
}
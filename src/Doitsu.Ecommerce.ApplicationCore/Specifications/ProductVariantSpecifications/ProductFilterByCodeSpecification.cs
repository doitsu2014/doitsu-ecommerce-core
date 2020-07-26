using System.Linq;
using Doitsu.Ecommerce.ApplicationCore.Entities;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.ProductVariantSpecifications
{
    public class ProductFilterByCodeSpecification : BaseSpecification<Products>
    {
        public ProductFilterByCodeSpecification(string code)
        {
            AddCriteria(p => p.Code == code);
            AddInclude("ProductOptions.ProductOptionValues");
            AddInclude("ProductVariants.ProductVariantOptionValues");
        }
    }
}
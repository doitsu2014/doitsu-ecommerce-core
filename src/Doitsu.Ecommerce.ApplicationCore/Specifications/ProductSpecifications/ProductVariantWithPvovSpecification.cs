using Doitsu.Ecommerce.ApplicationCore.Entities;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.ProductSpecifications
{
    public class ProductVariantWithPvovSpecification : BaseSpecification<ProductVariants>
    {

        public ProductVariantWithPvovSpecification()
        {
            AddInclude(pv => pv.ProductVariantOptionValues);
        }
        
    }
}
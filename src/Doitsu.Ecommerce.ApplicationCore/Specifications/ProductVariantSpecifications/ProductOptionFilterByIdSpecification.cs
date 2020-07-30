using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.QueryExtensions.Include;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.ProductVariantSpecifications
{
    public class ProductOptionFilterByIdSpecification : BaseSpecification<ProductOptions>
    {
        public ProductOptionFilterByIdSpecification(int id)
        {
            AddCriteria(po => po.Id == id);
            AddIncludes(po => po
                .Include(qPo => qPo.ProductOptionValues)
                .ThenInclude(qPov => qPov.ProductVariantOptionValues)
                .ThenInclude(qPvov => qPvov.ProductVariant));
        }
        
    }
}
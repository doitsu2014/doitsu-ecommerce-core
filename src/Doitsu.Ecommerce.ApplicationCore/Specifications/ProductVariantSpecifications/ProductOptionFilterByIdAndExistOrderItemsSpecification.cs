using System.Linq;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore.QueryExtensions.Include;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.ProductVariantSpecifications
{
    public class ProductOptionFilterByIdAndExistOrderItemsSpecification : BaseSpecification<ProductOptions>
    {
        public ProductOptionFilterByIdAndExistOrderItemsSpecification(int id)
        {
            AddIncludes(po => po
                .Include(qPo => qPo.ProductOptionValues)
                .ThenInclude(qPov => qPov.ProductVariantOptionValues)
                .ThenInclude(qPvov => qPvov.ProductVariant)
                .ThenInclude(qPv => qPv.OrderItems));

            AddCriteria(po => po.Id == id 
                && po.ProductOptionValues.Count > 0 
                && po.ProductOptionValues
                        .Where(pov => pov.ProductVariantOptionValues.Count() > 0)
                        .Any(pov => pov.ProductVariantOptionValues.Any(pvov => pvov.ProductVariant.OrderItems.Count() > 0)));
        }
        
    }
}
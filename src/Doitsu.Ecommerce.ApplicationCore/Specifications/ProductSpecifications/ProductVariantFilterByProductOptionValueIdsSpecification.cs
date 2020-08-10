using System.Linq;
using Doitsu.Ecommerce.ApplicationCore.Entities;

namespace Doitsu.Ecommerce.ApplicationCore.Specifications.ProductSpecifications
{
    public class ProductVariantFilterByProductOptionValueIdsSpecification : BaseSpecification<ProductVariants>
    {
        public ProductVariantFilterByProductOptionValueIdsSpecification(int[] povIds)
        {
            var lengthOfListPovIds = povIds.Length; 
            AddCriteria(pv => pv.ProductVariantOptionValues.Count == lengthOfListPovIds 
                && pv.ProductVariantOptionValues.Select(pvov => pvov.ProductOptionValueId ?? int.MinValue)
                    .All(pvovPovId => povIds.Contains(pvovPovId)));
        }

        public ProductVariantFilterByProductOptionValueIdsSpecification(int[][] arrayOfArrayPovIds)
        {
            var povIds = arrayOfArrayPovIds.SelectMany(x => x);
            AddCriteria(pv => pv.ProductVariantOptionValues.Select(pvov => pvov.ProductOptionValueId ?? int.MinValue)
                    .All(pvovPovId => povIds.Contains(pvovPovId)));
        }

        public ProductVariantFilterByProductOptionValueIdsSpecification((int productId, int[] povIds)[] filterParams)
        {
            var productIds = filterParams.Select(x => x.productId).ToArray();
            var povIds = filterParams.SelectMany(x => x.povIds).ToArray();

            AddCriteria(pv => 
                productIds.Contains(pv.ProductId)
                && pv.ProductVariantOptionValues.Select(pvov => pvov.ProductOptionValueId ?? int.MinValue)
                    .All(pvovPovId => povIds.Contains(pvovPovId)));
        }

    }
}
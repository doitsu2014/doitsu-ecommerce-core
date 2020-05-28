using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Doitsu.Ecommerce.Core.SEO.Interfaces;
using Schema.NET;

namespace Doitsu.Ecommerce.Core.SEO.Services
{
    public class ProductGenerator : IProductGenerator
    {
        public Product GenerateFromProductDetail(ProductDetailViewModel productDetailViewModel)
        {
            var result = new Product();
            return null;
        }
    }
}
using Doitsu.Ecommerce.Core.Abstraction.ViewModels;
using Schema.NET;
using System.Collections.Generic;

namespace Doitsu.Ecommerce.Core.SEO.Interfaces
{
    public interface IProductGenerator
    {
       List<Product> GenerateProductFromProductDetail(ProductDetailViewModel productDetailViewModel,Organization organization);
       string GenerateProductJsonLdFromProductDetail(ProductDetailViewModel productDetailViewModel, Organization organization);
    }
}
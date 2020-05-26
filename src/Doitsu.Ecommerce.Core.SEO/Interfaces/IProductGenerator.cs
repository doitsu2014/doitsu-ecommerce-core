using Doitsu.Ecommerce.Core.Abstraction.ViewModels;

namespace Doitsu.Ecommerce.Core.SEO.Interfaces
{
    public interface IProductGenerator
    {
       Schema.NET.Product GenerateFromProductDetail(ProductDetailViewModel productDetailViewModel);
    }
}
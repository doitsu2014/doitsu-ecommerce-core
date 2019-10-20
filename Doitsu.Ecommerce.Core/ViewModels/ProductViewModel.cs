using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Service.Core.Abstraction;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.ViewModels
{
    public class ProductViewModel : BaseViewModel<Products>
    {
        public int Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? CateId { get; set; }
        public int? ArtistId { get; set; }
        public int? CollectionId { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }
        public string ImageThumbUrl { get; set; }
        public string ImageUrls { get; set; }
        public decimal Price { get; set; }
        public string Property { get; set; }
        public string Slug { get; set; }
    }

    public class ProductDetailWrapperViewModel
    {
        public static ProductDetailWrapperViewModel Empty => new ProductDetailWrapperViewModel();
        [JsonProperty("product")]
        public ProductDetailViewModel Product { get; set; }
        [JsonProperty("categories")]
        public ImmutableList<CategoryViewModel> Categories { get; set; }
        [JsonProperty("parentCategorySlug")]
        public string ParentCategorySlug { get; set; }
    }

    public class ProductDetailViewModel : BaseViewModel<Products>
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("imageThumbUrl")]
        public string ImageThumbUrl { get; set; }
        [Required]
        [JsonProperty("name")]
        public string Name { get; set; }
        [Required]
        [JsonProperty("code")]
        public string Code { get; set; }
        [Required]
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [Required]
        [JsonProperty("slug")]
        public string Slug { get; set; }
        [JsonProperty("categorySlug")]
        public string CategorySlug { get; set; }
        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }
        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("imageUrls")]
        public string ImageUrls { get; set; }

        [JsonProperty("property")]
        public string Property { get; set; }
        [Required]
        [JsonProperty("cateId")]
        public int? CateId { get; set; }
    }



    public class ProductOverviewViewModel : BaseViewModel<Products>
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("imageThumbUrl")]
        public string ImageThumbUrl { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("code")]
        public string Code { get; set; }
        [JsonProperty("price")]
        public decimal Price { get; set; }
        [JsonProperty("slug")]
        public string Slug { get; set; }
        [JsonProperty("categorySlug")]
        public string CategorySlug { get; set; }
        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }
        [JsonProperty("categoryRecursive")]
        public CategoryRecursiveViewModel Cate { get; set; }
    }


    /// <summary>
    /// This view model to wrapper necessary data and pass to angularjs
    /// </summary>
    public class ListProductsWrapperViewModel
    {
        public static ListProductsWrapperViewModel Empty => new ListProductsWrapperViewModel();
        [JsonProperty("products")]
        public ImmutableList<ProductOverviewViewModel> Products { get; set; }
        [JsonProperty("categories")]
        public ImmutableList<CategoryViewModel> Categories { get; set; }
        [JsonProperty("parentCategorySlug")]
        public string ParentCategorySlug { get; set; }
        [JsonProperty("currentCateSlug")]
        public string CurrentCateSlug { get; set; }
    }

    public class OverviewBuildingProductsViewModel
    {
        public OverviewBuildingProductsViewModel()
        {
            ProductOverviews = ImmutableList<ProductOverviewViewModel>.Empty;
        }
        public static OverviewBuildingProductsViewModel Empty => new OverviewBuildingProductsViewModel();

        [JsonProperty("categoryVm")]
        public CategoryViewModel CategoryVm { get; set; }
        [JsonProperty("productOverviews")]
        public ImmutableList<ProductOverviewViewModel> ProductOverviews { get; set; }
    }
}


using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Service.Core.Abstraction;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.ViewModels
{
    public class ProductViewModel : BaseViewModel<Products>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Property { get; set; }
        public int? CateId { get; set; }
        public int? ArtistId { get; set; }
        public int? CollectionId { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }
        public string ImageThumbUrl { get; set; }
        public string ImageUrls { get; set; }
        public decimal Price { get; set; }
        public string Slug { get; set; }
        public int? Sku { get; set; }
        public float Weight { get; set; }
        public ICollection<ProductOptionViewModel> ProductOptions { get; set; }
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

        [Required]
        [JsonProperty("slug")]
        public string Slug { get; set; }
        
        [Required]
        [JsonProperty("weight")]
        public float Weight { get; set; }
    
        [JsonProperty("vers")]
        public byte[] Vers { get; set; }

        [JsonProperty("productOptions")]
        public ICollection<ProductOptionViewModel> ProductOptions { get; set; }

        [JsonProperty("productVariants")]
        public ICollection<ProductVariantViewModel> ProductVariants { get; set; }
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
        [JsonProperty("productOptions")]
        public ICollection<ProductOptionViewModel> ProductOptions { get; set; }
        [JsonProperty("categoryRecursive")]
        public CategoryWithParentViewModel Cate { get; set; }
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

    public class CreateProductViewModel : ProductDetailViewModel
    {
    }

    public class UpdateProductViewModel : ProductDetailViewModel
    {
    }

    public class ProductVariantViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("productId")]
        public int ProductId { get; set; }
        [JsonProperty("sku")]
        public string Sku { get; set; }
        [JsonProperty("anotherPrice")]
        public decimal AnotherPrice { get; set; }
        [JsonProperty("anotherDiscount")]
        public float AnotherDiscount { get; set; }
        [JsonProperty("inventoryQuantity")]
        public long InventoryQuantity { get; set; }
        [JsonProperty("vers")]
        public byte[] Vers { get; set; }
        [JsonProperty("status")]
        public ProductVariantStatusEnum Status { get; set; }

        [JsonProperty("productPrice")]
        public decimal ProductPrice { get; set; }

        [JsonProperty("productWeight")]
        public float ProductWeight { get; set; }

        [JsonProperty("productVariantOptionValues")]
        public virtual ICollection<ProductVariantOptionValueViewModel> ProductVariantOptionValues { get; set; }
    }

    public class ProductVariantDetailViewModel : ProductVariantViewModel
    {
    }

    public class ProductVariantOptionValueViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("productOptionId")]
        public int ProductOptionId { get; set; }
        [JsonProperty("productVariantId")]
        public int ProductVariantId { get; set; }
        [JsonProperty("productOptionValueId")]
        public int ProductOptionValueId { get; set; }
        [JsonProperty("Vers")]
        public byte[] Vers { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("productOption")]
        public ProductOptionViewModel ProductOption { get; set; }
        [JsonProperty("productOptionValue")]
        public ProductOptionValueViewModel ProductOptionValue { get; set; }
    }

    public class ProductFilterParamViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("productOptions")]
        public ProductOptionFilterParamViewModel[] ProductOptions { get; set; }
    }

    public class ProductOptionFilterParamViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("selectedValueId")]
        public int? SelectedValueId { get; set; }
    }
}


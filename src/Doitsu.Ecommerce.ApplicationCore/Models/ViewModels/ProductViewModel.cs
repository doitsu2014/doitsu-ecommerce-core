using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.ApplicationCore.Models.ViewModels
{
    public class ProductViewModel : BaseViewModel<Products>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
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

    public class ProductOverviewViewModel
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
        public string CateSlug { get; set; }

        [JsonProperty("categoryName")]
        public string CateName { get; set; }

        [JsonProperty("shortDescription")]
        public string ShortDescription { get; set; }

        [JsonProperty("imageUrls")]
        public string ImageUrls { get; set; }

        [Required]
        [JsonProperty("cateId")]
        public int? CateId { get; set; }

        [Required]
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [Required]
        [JsonProperty("weight")]
        public float Weight { get; set; }

        [JsonProperty("inventoryQuantity")]
        public long InventoryQuantity { get; set; }

        [JsonProperty("vers")]
        public byte[] Vers { get; set; }

        [JsonProperty("productOptions")]
        public ICollection<ProductOptionViewModel> ProductOptions { get; set; }

        [JsonProperty("productVariants")]
        public ICollection<ProductVariantViewModel> ProductVariants { get; set; }

        [JsonProperty("categoryRecursive")]
        public CategoryWithParentViewModel Cate { get; set; }

        [JsonProperty("lastUpdatedDate")]
        public DateTime? LastUpdatedDate { get; set; }

        [JsonProperty("createdDate")]
        public DateTime? CreatedDate { get; set; }
    }

    public class ProductDetailViewModel : ProductOverviewViewModel
    {
        [JsonProperty("description")]
        public string Description { get; set; }
    }

    public class CreateProductViewModel : ProductDetailViewModel
    {
    }

    public class UpdateProductViewModel : ProductDetailViewModel
    {
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

    public class ProductFilterParamViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("productOptions")]
        public ProductOptionFilterParamViewModel[] ProductOptions { get; set; }
    }
}


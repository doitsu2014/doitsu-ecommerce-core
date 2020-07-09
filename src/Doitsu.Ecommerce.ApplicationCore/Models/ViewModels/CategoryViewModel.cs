using System.Linq;
using System.Collections.Generic;
using System.Collections.Immutable;
using AutoMapper;
using Doitsu.Ecommerce.ApplicationCore.Entities;
using Doitsu.Ecommerce.ApplicationCore;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.ApplicationCore.Models.ViewModels
{
    public class SimpleCategoryViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }
        [JsonProperty("slug")]
        public string Slug { get; set; }
        [JsonProperty("isFixed")]
        public bool IsFixed { get; set; }
        [JsonProperty("parentCateId")]
        public int? ParentCateId { get; set; }
        [JsonProperty("parentCateName")]
        public string ParentCateName { get; set; }
        [JsonProperty("parentCateSlug")]
        public string ParentCateSlug { get; set; }
        [JsonProperty("parentCateIsFixed")]
        public bool ParentCateIsFixed { get; set; }
        [JsonProperty("vers")]
        public byte[] Vers { get; set; }
    }

    /// <summary>
    /// This is origin view model of Categories Entity
    /// And this is default return type of Categories Services
    /// </summary>
    public class CategoryViewModel : BaseViewModel<Categories>
    {
        public CategoryViewModel()
        {
        }

        public CategoryViewModel(Categories entity, IMapper mapper) : base(entity, mapper)
        {
        }

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }
        [JsonProperty("slug")]
        public string Slug { get; set; }
        [JsonProperty("isFixed")]
        public bool IsFixed { get; set; }
        [JsonProperty("parentCateId")]
        public int? ParentCateId { get; set; }
        [JsonProperty("parentCateName")]
        public string ParentCateName { get; set; }
        [JsonProperty("parentCateSlug")]
        public string ParentCateSlug { get; set; }
        [JsonProperty("vers")]
        public byte[] Vers { get; set; }
    }

    public class CategoryWithInverseParentViewModel : SimpleCategoryViewModel
    {
        [JsonProperty("inverseParentCate")]
        public ICollection<CategoryWithInverseParentViewModel> InverseParentCate { get; set; }
    }

    public class CategoryWithParentViewModel : SimpleCategoryViewModel
    {
        [JsonProperty("parentCate")]
        public CategoryWithoutParentViewModel ParentCate { get; set; }
    }

    public class CategoryWithoutParentViewModel : SimpleCategoryViewModel
    {
    }

    public class BaseCategoryViewModel : SimpleCategoryViewModel
    {
    }

    public class CategoryMenuViewModel : SimpleCategoryViewModel
    {
        public ICollection<CategoryViewModel> InverseParentCate { get; set; }
    }

    public class CategoryWithProductOverviewViewModel : SimpleCategoryViewModel
    {
        [JsonProperty("inverseParentCate")]
        public ICollection<CategoryWithProductOverviewViewModel> InverseParentCate { get; set; }

        [JsonProperty("products")]
        public ICollection<ProductOverviewViewModel> Products { get; set; }
    }

    public class CategoryAndListProductOverviewViewModel
    {
        public CategoryWithoutParentViewModel Cate { get; set; }
        public ImmutableList<ProductOverviewViewModel> Products { get; set; }
    }
}

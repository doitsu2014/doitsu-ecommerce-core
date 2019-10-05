using System.Collections.Generic;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Service.Core.Abstraction;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.ViewModels
{
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

    }

    public class CategoryRecursiveViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }
        [JsonProperty("slug")]
        public string Slug { get; set; }
        [JsonProperty("parentCate")]
        public CategoryRecursiveViewModel ParentCate { get; set; }
    }

    public class CategoryMenuViewModel : BaseViewModel<Categories>
    {
        public CategoryMenuViewModel()
        {
        }

        public CategoryMenuViewModel(Categories entity, IMapper mapper) : base(entity, mapper)
        {
        }

        public int Id { get; set; }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string Slug { get; set; }
        public bool IsFixed { get; set; }
        public int? ParentCateId { get; set; }

        public ICollection<CategoryViewModel> InverseParentCate { get; set; }

    }

    public class CategoryWithProductOverviewViewModel : BaseViewModel<Categories>
    {
        public CategoryWithProductOverviewViewModel()
        {
        }

        public CategoryWithProductOverviewViewModel(Categories entity, IMapper mapper) : base(entity, mapper)
        {
        }
        public int Id { get; set; }
        public string Slug { get; set; }

        public ICollection<CategoryWithProductOverviewViewModel> InverseParentCate { get; set; }
        public ICollection<ProductOverviewViewModel> Products { get; set; }
    }
}

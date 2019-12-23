using System.Collections.Generic;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.ViewModels
{

    public class ProductOptionViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("name")]
        public string Name { get; set; }
        [JsonProperty("productId")]
        public int ProductId { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }
        [JsonProperty("vers")]
        public byte[] Vers { get; set; }
        [JsonProperty("productOptionValues")]
        public virtual ICollection<ProductOptionValueViewModel> ProductOptionValues { get; set; }
    }

    public class ProductOptionValueViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("productOptionId")]
        public int ProductOptionId { get; set; }
        [JsonProperty("value")]
        public string Value { get; set; }
        [JsonProperty("active")]
        public bool Active { get; set; }
        [JsonProperty("status")]
        public ProductOptionValueStatusEnum Status { get; set; }
        [JsonProperty("vers")]
        public byte[] Vers { get; set; }
    }
}


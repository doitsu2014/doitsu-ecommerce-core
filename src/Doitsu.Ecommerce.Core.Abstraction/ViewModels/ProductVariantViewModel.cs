using System.Collections.Generic;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.Abstraction.ViewModels
{
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

        [JsonProperty("inventoryStatus")]
        public ProductVariantInventoryStatusEnum InventoryStatus { get; set; }

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
}


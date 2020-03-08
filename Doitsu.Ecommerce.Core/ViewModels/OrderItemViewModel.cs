using System.Collections.Generic;
using AutoMapper;
using Doitsu.Ecommerce.Core.Data.Entities;
using Doitsu.Service.Core;
using Doitsu.Service.Core.Abstraction;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.ViewModels
{
    public class OrderItemViewModel : BaseViewModel<OrderItems>
    {
        public OrderItemViewModel()
        {
        }

        public OrderItemViewModel(OrderItems entity, IMapper mapper) : base(entity, mapper)
        {
        }

        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("productId")]
        public int ProductId { get; set; }
        [JsonProperty("productVariantId")]
        public int? ProductVariantId { get; set; }
        [JsonProperty("orderId")]
        public int OrderId { get; set; }
        [JsonProperty("subTotalPrice")]
        public decimal SubTotalPrice { get; set; }
        [JsonProperty("subTotalQuantity")]
        public int SubTotalQuantity { get; set; }
        [JsonProperty("discount")]
        public double? Discount { get; set; }
        [JsonProperty("subTotalFinalPrice")]
        public decimal SubTotalFinalPrice { get; set; }
        [JsonProperty("productName")]
        public string ProductName { get; set; }
        [JsonProperty("productVariant")]
        public ProductVariantViewModel ProductVariant { get; set; }
    }
    public class CreateOrderItemWithOptionViewModel : OrderItemViewModel 
    {
        [JsonProperty("productOptionValues")]
        public ICollection<ProductOptionValueViewModel> ProductOptionValues {get;set;}
    }
}
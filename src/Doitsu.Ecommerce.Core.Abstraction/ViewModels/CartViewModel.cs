using System.Collections.Generic;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.Abstraction.ViewModels
{
    public class CartViewModel
    {

    }

    public class CheckoutCartViewModel
    {
        [JsonProperty("cartItems")]
        public List<CartItemViewModel> CartItems { get; set; }
        [JsonProperty("totalQuantity")]
        public int TotalQuantity { get; set; }
        [JsonProperty("totalPrice")]
        public int TotalPrice { get; set; }
    }

    public class CartItemViewModel
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
        public int Price { get; set; }
        [JsonProperty("slug")]
        public string Slug { get; set; }
        [JsonProperty("categorySlug")]
        public string CategorySlug { get; set; }
        [JsonProperty("categoryName")]
        public string CategoryName { get; set; }
        [JsonProperty("subTotal")]
        public int SubTotal { get; set; }

        [JsonProperty("subQuantity")]
        public int SubQuantity { get; set; }
    }
}
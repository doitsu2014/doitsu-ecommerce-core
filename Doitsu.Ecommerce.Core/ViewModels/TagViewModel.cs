using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.ViewModels
{
    public class TagViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("slug")]
        public string Slug { get; set; }
    }
}
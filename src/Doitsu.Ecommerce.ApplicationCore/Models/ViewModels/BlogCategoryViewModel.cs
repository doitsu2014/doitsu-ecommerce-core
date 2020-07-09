using Newtonsoft.Json;

namespace Doitsu.Ecommerce.ApplicationCore.Models.ViewModels
{
    public class BlogCategoryViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("slug")]
        public string Slug { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

    }

}
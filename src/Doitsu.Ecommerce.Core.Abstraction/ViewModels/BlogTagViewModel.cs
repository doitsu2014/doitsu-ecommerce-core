using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.Abstraction.ViewModels
{
    public class BlogTagViewModel
    {
        [JsonProperty("id")]
        public int Id { get; set; }
        [JsonProperty("tagId")]
        public int TagId { get; set; }
        [JsonProperty("tagTitle")]
        public string TagTitle { get; set; }
        [JsonProperty("tagSlug")]
        public string TagSlug { get; set; }
    }
}
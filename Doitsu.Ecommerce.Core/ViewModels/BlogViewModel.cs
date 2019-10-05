using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Doitsu.Ecommerce.Core.ViewModels
{
    public class BlogOverviewViewModel {
        [JsonProperty ("id")]
        public int Id { get; set; }

        [JsonProperty ("title")]
        public string Title { get; set; }

        [JsonProperty ("shortContent")]
        public string ShortContent { get; set; }

        [JsonProperty ("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty ("slug")]
        public string Slug { get; set; }

        [JsonProperty ("publishedTime")]
        public DateTime? PublishedTime { get; set; }

        [JsonProperty ("publisherFullname")]
        public string PublisherFullname { get; set; }

        [JsonProperty ("createrFullname")]
        public string CreaterFullname { get; set; }

        [JsonProperty ("blogCategory")]
        public BlogCategoryViewModel BlogCategory { get; set; }
    }
    public class BlogDetailViewModel {
        [JsonProperty ("id")]
        public int Id { get; set; }

        [JsonProperty ("content")]
        public string Content { get; set; }

        [JsonProperty ("shortContent")]
        public string ShortContent { get; set; }

        [JsonProperty ("title")]
        public string Title { get; set; }

        [JsonProperty ("thumbnailUrl")]
        public string ThumbnailUrl { get; set; }

        [JsonProperty ("slug")]
        public string Slug { get; set; }

        [JsonProperty ("publishedTime")]
        public DateTime? PublishedTime { get; set; }

        [JsonProperty ("publisherFullname")]
        public string PublisherFullname { get; set; }

        [JsonProperty ("createrFullname")]
        public string CreaterFullname { get; set; }

        [JsonProperty ("blogTags")]
        public ICollection<BlogTagViewModel> BlogTags { get; set; }

        [JsonProperty ("blogCategoryId")]
        public int BlogCategoryId { get; set; }

        [JsonProperty ("blogCategory")]
        public BlogCategoryViewModel BlogCategory { get; set; }
    }
}
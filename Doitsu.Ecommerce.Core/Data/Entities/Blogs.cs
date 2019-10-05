using System;
using System.Collections.Generic;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class Blogs : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Slug { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime DraftedTime { get; set; }
        public DateTime? PublishedTime { get; set; }
        public int BlogCategoryId { get; set; }
        public int? PublisherId { get; set; }
        public int CreaterId { get; set; }
        public string ThumbnailUrl { get; set; }
        public string ImageUrls { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }
        public string ShortContent { get; set; }

        public virtual BlogCategories BlogCategory { get; set; }
        public virtual EcommerceIdentityUser Creater { get; set; }
        public virtual EcommerceIdentityUser Publisher { get; set; }
        public virtual ICollection<BlogTags> BlogTags { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
{
    public class BlogCategories : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Name { get; set; }
        public int? Position { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? UpdatedTime { get; set; }
        public bool Active { get; set; }
        public int? BlogCategoryId { get; set; }
        public string Slug { get; set; }
        public byte[] Vers { get; set; }

        public virtual BlogCategories BlogCategory { get; set; }
        public virtual ICollection<Blogs> Blogs { get; set; }
        public virtual ICollection<BlogCategories> InverseBlogCategory { get; set; }
    }
}

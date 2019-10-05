using System.Collections.Generic;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class Tag : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Title { get; set; }
        public bool Active { get; set; }
        public string Slug { get; set; }
        public byte[] Vers { get; set; }

        public virtual ICollection<BlogTags> BlogTags { get; set; }
        public virtual ICollection<ProductTag> ProductTag { get; set; }
    }
}

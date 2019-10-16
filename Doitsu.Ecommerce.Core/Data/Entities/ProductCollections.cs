using System.Collections.Generic;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class ProductCollections : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Slug { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }

        public virtual Categories ParentCate { get; set; }
        public virtual ICollection<Categories> InverseParentCate { get; set; }
        public virtual ICollection<Products> Products { get; set; }
    }
}

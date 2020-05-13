using System.Collections.Generic;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Abstraction.Entities
{
    public class Categories : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Name { get; set; }
        public bool Active { get; set; }
        public int? ParentCateId { get; set; }
        public bool IsFixed { get; set; }
        public byte[] Vers { get; set; }
        public string Slug { get; set; }

        public virtual Categories ParentCate { get; set; }
        public virtual ICollection<Categories> InverseParentCate { get; set; }
        public virtual ICollection<Products> Products { get; set; }
    }
}

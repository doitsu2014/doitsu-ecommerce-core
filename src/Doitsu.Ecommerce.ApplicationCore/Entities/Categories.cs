using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
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

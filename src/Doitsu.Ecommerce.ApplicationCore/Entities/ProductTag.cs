using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
{
    public class ProductTag : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public int TagId { get; set; }
        public int ProductId { get; set; }
        public byte[] Vers { get; set; }
        public bool Active { get; set; }

        public virtual Products Product { get; set; }
        public virtual Tag Tag { get; set; }
    }
}

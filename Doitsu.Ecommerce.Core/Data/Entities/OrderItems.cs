using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class OrderItems : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public int ProductId { get; set; }
        public int OrderId { get; set; }
        public decimal SubTotalPrice { get; set; }
        public int SubTotalQuantity { get; set; }
        public double? Discount { get; set; }
        public decimal SubTotalFinalPrice { get; set; }
        public byte[] Vers { get; set; }
        public bool Active { get; set; }

        public virtual Orders Order { get; set; }
        public virtual Products Product { get; set; }
    }
}

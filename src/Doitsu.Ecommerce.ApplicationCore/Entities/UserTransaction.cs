using System;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
{
    public class UserTransaction : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public UserTransactionTypeEnum Type {get;set;}
        public DateTime CreatedTime { get; set; }
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public UserTransactionSignEnum Sign { get; set; }
        public decimal DestinationBalance { get; set; }
        public decimal CurrentBalance { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }

        public virtual Orders Order { get; set; }
        public virtual EcommerceIdentityUser User { get; set; }
    }
}

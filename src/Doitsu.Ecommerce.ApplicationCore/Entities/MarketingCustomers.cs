using System;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
{
    public class MarketingCustomers : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Email { get; set; }
        public int? UserId { get; set; }
        public DateTime JoinedDate { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }

        public virtual EcommerceIdentityUser User { get; set; }
    }
}

using System;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
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

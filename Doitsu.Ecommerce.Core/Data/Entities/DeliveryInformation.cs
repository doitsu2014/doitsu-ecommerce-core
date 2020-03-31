using System.Collections;
using System.Collections.Generic;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class DeliveryInformation : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public int UserId { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Country { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }

        public EcommerceIdentityUser User { get; set; }
        public ICollection<Orders> Orders {get;set;}
    }
}
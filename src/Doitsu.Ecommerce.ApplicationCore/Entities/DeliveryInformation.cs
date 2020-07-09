using System.Collections;
using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore.Entities.Identities;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
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
        public string District { get; set; }
        public string Ward { get; set; }
        public string ZipCode { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }

        public EcommerceIdentityUser User { get; set; }
        public ICollection<Orders> Orders {get;set;}
    }
}
using System;
using System.Collections.Generic;
using Doitsu.Ecommerce.Core.Data.Identities;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class Orders : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Code { get; set; }
        public double Discount { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal FinalPrice { get; set; }
        public int UserId { get; set; }
        public int TotalQuantity { get; set; }
        public int Status { get; set; }
        public bool Active { get; set; }
        public DateTime CreatedDate { get; set; }
        public string DeliveryAddress { get; set; }
        public string DeliveryName { get; set; }
        public string DeliveryPhone { get; set; }
        public string DeliveryEmail { get; set; }
        public byte[] Vers { get; set; }

        public virtual EcommerceIdentityUser User { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }
    }
}

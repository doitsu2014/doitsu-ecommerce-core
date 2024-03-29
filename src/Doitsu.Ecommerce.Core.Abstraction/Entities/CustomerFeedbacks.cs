﻿using System;
using Doitsu.Ecommerce.Core.Abstraction.Identities;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Abstraction.Entities
{
    public class CustomerFeedbacks : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string CustomerName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Content { get; set; }
        public int? UserId { get; set; }
        public DateTime CreatedDate { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }
        public string Address { get; set; }
        public int Type { get; set; }

        public virtual EcommerceIdentityUser User { get; set; }
    }
}

using System;
using System.Collections.Generic;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Abstraction.Entities
{

    public class WareHouse : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public int BrandId { get; set; }
        public string Name { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Ward { get; set; }
        public string Address { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }

        public Brand Brand { get; set; }
    }
}

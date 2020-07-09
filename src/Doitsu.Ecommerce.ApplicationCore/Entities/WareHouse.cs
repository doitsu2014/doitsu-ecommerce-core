using System;
using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
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

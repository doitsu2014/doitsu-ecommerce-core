﻿using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
{
    public class Tag : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Title { get; set; }
        public bool Active { get; set; }
        public string Slug { get; set; }
        public byte[] Vers { get; set; }

        public virtual ICollection<BlogTags> BlogTags { get; set; }
        public virtual ICollection<ProductTag> ProductTag { get; set; }
    }
}
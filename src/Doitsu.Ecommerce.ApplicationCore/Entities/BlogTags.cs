﻿using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
{
    public class BlogTags : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public int BlogId { get; set; }
        public int TagId { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }

        public virtual Blogs Blog { get; set; }
        public virtual Tag Tag { get; set; }
    }
}

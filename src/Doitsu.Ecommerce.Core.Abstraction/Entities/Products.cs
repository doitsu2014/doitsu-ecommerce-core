﻿using System;
using System.Collections.Generic;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Abstraction.Entities
{
    public class Products : Entity<int>, IConcurrencyCheckVers, IActivable, IAuditable
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ShortDescription { get; set; }
        public int? CateId { get; set; }
        public int? ArtistId { get; set; }
        public int? CollectionId { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }
        public string ImageThumbUrl { get; set; }
        public string ImageUrls { get; set; }
        public decimal Price { get; set; }
        public string Slug { get; set; }
        public float Weight { get; set; }
        public long InventoryQuantity { get; set; }
        public DateTime? LastUpdatedDate { get; set; }
        public DateTime? CreatedDate { get; set; }

        public virtual Categories Cate { get; set; }
        public virtual ICollection<OrderItems> OrderItems { get; set; }
        public virtual ICollection<ProductTag> ProductTag { get; set; }
        public virtual ICollection<ProductCollectionMappings> ProductCollectionMappings { get; set; }
        public virtual ICollection<ProductVariants> ProductVariants { get; set; }
        public virtual ICollection<ProductOptions> ProductOptions { get; set; }
    }
}

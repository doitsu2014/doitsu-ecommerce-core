using System.Collections.Generic;
using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class ProductCollectionMappings : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public int ProductCollectionId { get; set; }
        public int ProductId { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }
        
        public virtual Products Product { get; set; }
        public virtual ProductCollections ProductCollection { get; set; }
    }
}

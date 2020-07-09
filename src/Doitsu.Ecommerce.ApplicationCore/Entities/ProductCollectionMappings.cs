using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
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

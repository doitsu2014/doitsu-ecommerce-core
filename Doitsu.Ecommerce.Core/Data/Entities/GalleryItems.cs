using Doitsu.Service.Core.Interfaces.EfCore;
using EFCore.Abstractions.Models;

namespace Doitsu.Ecommerce.Core.Data.Entities
{
    public class GalleryItems : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Slug { get; set; }
        public int GalleryId { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }

        public virtual Galleries Gallery { get; set; }
    }
}

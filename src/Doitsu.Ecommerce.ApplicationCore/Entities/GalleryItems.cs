using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
{
    public class GalleryItems : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Title { get; set; }
        public string ImageUrl { get; set; }
        public string Slug { get; set; }
        public string Link { get; set; }
        public int GalleryId { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }

        public virtual Galleries Gallery { get; set; }
    }
}

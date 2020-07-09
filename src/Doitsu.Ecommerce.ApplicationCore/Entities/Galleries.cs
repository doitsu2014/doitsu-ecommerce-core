using System.Collections.Generic;
using Doitsu.Ecommerce.ApplicationCore.Interfaces.Data;
using Doitsu.Ecommerce.ApplicationCore;

namespace Doitsu.Ecommerce.ApplicationCore.Entities
{
    public class Galleries : Entity<int>, IConcurrencyCheckVers, IActivable
    {
        public string Title { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Slug { get; set; }
        public int? ParentGalleryId { get; set; }
        public bool Active { get; set; }
        public byte[] Vers { get; set; }

        public virtual Galleries ParentGallery { get; set; }
        public virtual ICollection<GalleryItems> GalleryItems { get; set; }
        public virtual ICollection<Galleries> InverseParentGallery { get; set; }
    }
}
